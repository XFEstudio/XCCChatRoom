using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.ArrayExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewModel;

public partial class CommunityPageViewModel(CommunityPage viewPage) : ObservableObject
{
    private readonly List<PostCardView> postCardList = [];
    private readonly List<string> postIdList = [];
    internal long totalHeight = 0;
    private bool firstRefresh = true;
    private bool tapped = false;
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();

    private bool refreshingIsBusy = false;
    internal bool RefreshingIsBusy
    {
        get => refreshingIsBusy;
        set
        {
            refreshingIsBusy = value;
            if (value)
            {
                ViewPage.loadingLabel.IsVisible = true;
            }
            else
            {
                ViewPage.loadingLabel.IsVisible = false;
            }
        }
    }

    public CommunityPage ViewPage { get; init; } = viewPage;

    [RelayCommand]
    async Task PostRefresh()
    {
        var postDataList = await AppAlgorithm.GetLatestPost(20);
        if (postDataList is not null)
            foreach (var postData in postDataList)
            {
                var tarPostCard = postCardList.Find(x => x.PostId == postData.PostID);
                if (tarPostCard is not null)
                {
                    tarPostCard.Dispatcher.Dispatch(() =>
                    {
                        tarPostCard.ReloadData(postData);
                        if (UserInfoProfile.LoginSuccessful && UserInfoProfile.CurrentUser.LikedPostID.Contains(tarPostCard.PostId))
                        {
                            tarPostCard.IsLike = true;
                        }
                    });
                    totalHeight = GetTotalHeight();
                }
                else
                {
                    PostCardView post;
                    if (firstRefresh)
                        post = new PostCardView(postData, false)
                        {
                            Margin = new Thickness(0, 3, 0, 3)
                        };
                    else
                        post = new PostCardView(postData)
                        {
                            Margin = new Thickness(0, 3, 0, 3)
                        };
                    post.LikeClick += Post_LikeClick;
                    post.Click += Post_Click;
                    post.TagClick += Post_TagClick;
                    if (UserInfoProfile.LoginSuccessful && UserInfoProfile.CurrentUser.LikedPostID.Contains(postData.PostID))
                    {
                        post.IsLike = true;
                    }
                    postCardList.Add(post);
                    postIdList.Add(post.PostId);
                    ViewPage.postRefreshView.Dispatcher.Dispatch(() =>
                    {
                        ViewPage.postStackLayout.Children.Insert(0, post);
                    });
                    totalHeight = GetTotalHeight();
                    Trace.WriteLine($"滚动：{ViewPage.postScrollView.Height}\t当前：{totalHeight}");
                    if (totalHeight > ViewPage.postScrollView.Height && firstRefresh)
                        break;
                }
            }
        ViewPage.postRefreshView.Dispatcher.Dispatch(() =>
        {
            ViewPage.postRefreshView.IsRefreshing = false;
        });
        totalHeight = GetTotalHeight();
        firstRefresh = false;
    }

    internal async void GetDownPost()
    {
        var postDataList = await AppAlgorithm.GetElderPost(3, postIdList);
        if (postDataList is not null)
            foreach (var postData in postDataList)
            {
                var post = new PostCardView(postData);
                post.LikeClick += Post_LikeClick;
                post.Click += Post_Click;
                post.TagClick += Post_TagClick;
                if (UserInfoProfile.LoginSuccessful && UserInfoProfile.CurrentUser.LikedPostID.Contains(postData.PostID))
                {
                    post.IsLike = true;
                }
                postCardList.Add(post);
                postIdList.Add(postData.PostID);
                ViewPage.postRefreshView.Dispatcher.Dispatch(() =>
                {
                    ViewPage.postStackLayout.Children.Add(post);
                });
            }
        totalHeight = GetTotalHeight();
        RefreshingIsBusy = false;
    }

    public void RemovePostByID(string postId)
    {
        ViewPage.postStackLayout.Children.Remove(postCardList.Find(x => x.PostId == postId));
        totalHeight = GetTotalHeight();
    }

    private async void Post_TagClick(object sender, PostCardViewTagClickEventArgs e) => await Shell.Current?.DisplayAlert("屏蔽标签", "是否屏蔽标签：" + e.TagString, "屏蔽", "取消");

    private async void Post_Click(object sender, PostCardViewClickEventArgs e)
    {
        if (UserInfoProfile.LoginSuccessful)
        {
            await Shell.Current.GoToAsync($"{nameof(PostViewPage)}?PostID={e.PostEntity.PostID}");
        }
        else
        {
            if (await Shell.Current?.DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
        }
    }

    private async void Post_LikeClick(object sender, PostCardViewLikeClickEventArgs e)
    {
        var post = sender as PostCardView;
        if (!UserInfoProfile.LoginSuccessful)
        {
            post.LikeCount--;
            post.IsLike = false;
            if (await Shell.Current?.DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
            return;
        }
        if (e.IsLike)
        {
            e.PostEntity.PostLike++;
            UserInfoProfile.CurrentUser.LikedPostID += new string[] { e.PostEntity.PostID }.ToXFEString();
            await UserInfoPage.UpLoadUserInfo();
            if (await e.PostEntity.ExecuteUpdate(XFEExecuter) == 0)
            {
                e.PostEntity.PostLike--;
                post.LikeCount--;
                post.IsLike = false;
                await Shell.Current?.DisplayAlert("点赞失败", "请检查网络设置", "确定");
            }
        }
        else
        {
            if (e.PostEntity.PostLike >= 0)
                e.PostEntity.PostLike--;
            UserInfoProfile.CurrentUser.LikedPostID = UserInfoProfile.CurrentUser.LikedPostID.Replace($"[+-{e.PostEntity.PostID}-+]", string.Empty);
            await UserInfoPage.UpLoadUserInfo();
            if (await e.PostEntity.ExecuteUpdate(XFEExecuter) == 0)
            {
                e.PostEntity.PostLike++;
                post.LikeCount--;
                post.IsLike = true;
                await Shell.Current?.DisplayAlert("取消点赞失败", "请检查网络设置", "确定");
            }
        }
    }

    public void ChangeToUnLoginStyle()
    {
        foreach (var post in postCardList)
        {
            post.IsLike = false;
        }
    }

    [RelayCommand]
    async Task PostEdit()
    {
        if (tapped)
        {
            return;
        }
        await ViewPage.ellipse.ScaleTo(0.8, 100, Easing.CubicOut);
        var task = Shell.Current.GoToAsync(nameof(PostEditPage));
        await ViewPage.ellipse.ScaleTo(1, 100, Easing.CubicOut);
        tapped = false;
    }

    [RelayCommand]
    async Task AddPost()
    {
        if (tapped)
        {
            return;
        }
        tapped = true;
        await ViewPage.ellipse.ScaleTo(0.8, 100, Easing.CubicOut);
        var task = Shell.Current.GoToAsync(nameof(PostEditPage));
        await ViewPage.ellipse.ScaleTo(1, 100, Easing.CubicOut);
        tapped = false;
    }

    private long GetTotalHeight()
    {
        long totalHeight = 0;
        foreach (var post in postCardList)
        {
            totalHeight += (long)post.DesiredSize.Height + 6;
        }
        return totalHeight;
    }
}
