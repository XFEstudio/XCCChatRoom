using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPopup;
using System.Diagnostics;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.ArrayExtension;
using XFEExtension.NetCore.TaskExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewModel;

internal partial class PostViewPageViewModel(PostViewPage viewPage) : ObservableObject
{
    [ObservableProperty]
    private int likeCount;
    [ObservableProperty]
    private int starCount;
    [ObservableProperty]
    private string authorName;
    [ObservableProperty]
    private string authorNameFirstLetter;
    [ObservableProperty]
    private string titleText;
    [ObservableProperty]
    private string contentText;
    [ObservableProperty]
    private string postTime;
    [ObservableProperty]
    private string inputText;
    [ObservableProperty]
    private string quoteText;
    [ObservableProperty]
    private bool isLike;
    [ObservableProperty]
    private bool isStar;
    public PostViewPage ViewPage { get; init; } = viewPage;
    public XFEChatRoom_CommunityPost CurrentPostData { get; set; }
    private readonly List<string> commentIDList = [];
    private readonly List<CommentCardView> commentCardList = [];
    private bool editing = false;
    internal bool initialized = false;
    private bool refreshingIsBusy = false;
    private long totalHeight = 0;
    private string quoteID = string.Empty;
    internal readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();

    internal async void InitializePageData()
    {
        initialized = true;
        await Refresh();
        if (CurrentPostData.UID == UserInfoProfile.CurrentUser.ID)
        {
            ViewPage.ToolbarItems.Add(new ToolbarItem
            {
                Text = "编辑",
                Command = new Command(async () =>
                {
                    if (editing)
                    {
                        return;
                    }
                    editing = true;
                    await Shell.Current.GoToAsync(nameof(PostEditPage));
                    editing = false;
                })
            });
        }
    }

    private void CloseQuote()
    {
        quoteID = string.Empty;
        ViewPage.quoteBorder.IsVisible = false;
    }

    public async Task Refresh()
    {
        try
        {
            XFEExecuter.RefreshExecuter();
            LoadComment();
            CurrentPostData = await XFEExecuter.ExecuteGetFirst<XFEChatRoom_CommunityPost>(x => x.PostID == ViewPage.PostID);
            if (CurrentPostData is null)
            {
                await Shell.Current?.DisplayAlert("哦不", "无法获取帖子信息\n帖子ID：" + ViewPage.PostID, "啊？");
                return;
            }
            ViewPage.Title = "小窝：" + (CurrentPostData.PostTitle.Length > 10 ? CurrentPostData.PostTitle[..10] + "..." : CurrentPostData.PostTitle);
            TitleText = CurrentPostData.PostTitle;
            AuthorNameFirstLetter = CurrentPostData.UName[0].ToString();
            ContentText = CurrentPostData.PostContent;
            AuthorName = CurrentPostData.UName;
            PostTime = CurrentPostData.PostTime.ToString();
            LikeCount = CurrentPostData.PostLike;
            StarCount = CurrentPostData.PostStar;
            if (UserInfoProfile.LoginSuccessful)
            {
                IsLike = UserInfoProfile.CurrentUser.LikedPostID.Contains(CurrentPostData.PostID);
                IsStar = UserInfoProfile.CurrentUser.StarredPostID.Contains(CurrentPostData.PostID);
            }
            ViewPage.tagStackLayout.Clear();
            foreach (var tag in CurrentPostData.PostTag.ToXFEArray<string>())
            {
                var button = new Button
                {
                    Text = $"#{tag}",
                    BackgroundColor = Color.FromArgb("#F0ECFE"),
                    TextColor = Color.FromArgb("#512BD4"),
                    CornerRadius = 30,
                    Margin = new Thickness(5, 3, 0, 3)
                };
                button.Clicked += TagButton_Clicked;
                ViewPage.tagStackLayout.Children.Add(button);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current?.DisplayAlert("哦不", "无法获取帖子信息\n帖子ID：" + ViewPage.PostID + "\n" + ex.Message, "啊？");
        }
    }

    private async void LoadComment()
    {
        if (refreshingIsBusy)
            return;
        refreshingIsBusy = true;
        int getCommentRetryTime = 0;
        int getQuoteRetryTime = 0;
        await new Action(async () =>
        {
            using (var Executer = XCCDataBase.XFEDataBase.CreateExecuter())
            {
                List<XFEChatRoom_CommunityComment> commentDataList = [];
                while (getCommentRetryTime < 30)
                {
                    try
                    {
                        commentDataList = await AppAlgorithm.GetPostComment(ViewPage.PostID, 10, commentIDList);
                        getCommentRetryTime = 0;
                        break;
                    }
                    catch (Exception)
                    {
                        getCommentRetryTime++;
                        continue;
                    }
                }
                if (commentDataList is not null)
                {
                    if (commentDataList.Count > 0)
                    {
                        ViewPage.Dispatcher.Dispatch(() =>
                        {
                            ViewPage.noneCommentLabel.IsVisible = false;
                        });
                        foreach (var commentData in commentDataList)
                        {
                            var tarCommentCard = commentCardList.Find(x => x.CommentID == commentData.CommentID);
                            XFEChatRoom_CommunityComment quoteCommentData = null;
                            while (getQuoteRetryTime < 30)
                            {
                                try
                                {
                                    quoteCommentData = await Executer.ExecuteGetFirst<XFEChatRoom_CommunityComment>(x => x.CommentID == commentData.QuoteID);
                                    break;
                                }
                                catch (Exception)
                                {
                                    if (getQuoteRetryTime == 29)
                                    {
                                        quoteCommentData = null;
                                        await PopupAction.DisplayPopup(new ErrorPopup("加载失败", $"无法获取评论的引用列表"));
                                        break;
                                    }
                                    getQuoteRetryTime++;
                                    continue;
                                }
                            }
                            if (tarCommentCard is not null)
                            {
                                tarCommentCard.ReloadData(commentData, quoteCommentData);
                                tarCommentCard.Dispatcher.Dispatch(() =>
                                {
                                    if (UserInfoProfile.LoginSuccessful && UserInfoProfile.CurrentUser.LikedCommentID.Contains(tarCommentCard.CommentID))
                                    {
                                        tarCommentCard.IsLike = true;
                                    }
                                });
                            }
                            else
                            {
                                var commentCard = new CommentCardView(commentData, quoteCommentData)
                                {
                                    Margin = new Thickness(0, 5)
                                };
                                commentCard.LikeClick += CommentCard_LikeClick;
                                commentCard.QuoteClick += CommentCard_QuoteClick;
                                commentCard.CommentCardTapped += CommentCard_CommentCardTapped;
                                if (UserInfoProfile.LoginSuccessful && UserInfoProfile.CurrentUser.LikedCommentID.Contains(commentData.CommentID))
                                {
                                    commentCard.IsLike = true;
                                }
                                commentCardList.Add(commentCard);
                                commentIDList.Add(commentCard.CommentID);
                                ViewPage.commentStack.Dispatcher.Dispatch(() =>
                                {
                                    ViewPage.commentStack.Add(commentCard);
                                });
                                totalHeight = GetTotalHeight();
                            }
                        }
                    }
                    else
                    {
                        ViewPage.Dispatcher.Dispatch(() =>
                        {
                            ViewPage.noneCommentLabel.IsVisible = true;
                        });
                    }
                }
                else
                {
                    await PopupAction.DisplayPopup(new ErrorPopup("加载失败", $"无法获取评论列表"));
                }
            }
            refreshingIsBusy = false;
        }).StartNewTask();
    }

    private void SwitchToQuoteStyle(string quoteID)
    {
        this.quoteID = quoteID;
        var tarComment = commentCardList.Find(x => x.CommentID == quoteID);
        QuoteText = $"{tarComment.UserName}：{tarComment.CommentContent}";
        ViewPage.quoteBorder.IsVisible = true;
    }

    private long GetTotalHeight()
    {
        long totalHeight = 0;
        foreach (var commentCard in commentCardList)
        {
            totalHeight += (long)commentCard.DesiredSize.Height + 12;
        }
        return totalHeight;
    }

    private void CommentCard_CommentCardTapped(object sender, EventArgs e)
    {
        if (sender is CommentCardView commentCard)
        {
            SwitchToQuoteStyle(commentCard.CommentID);
            ViewPage.inputEditor.Focus();
        }
    }

    private void CommentCard_QuoteClick(object sender, CommentCardQuoteClickEventArgs e)
    {
        if (sender is CommentCardView commentCard)
        {
            SwitchToQuoteStyle(commentCard.CommentID);
            ViewPage.inputEditor.Focus();
        }
    }

    private async void CommentCard_LikeClick(object sender, CommentCardLikeClickEventArgs e)
    {
        if (!UserInfoProfile.LoginSuccessful)
        {
            if (await Shell.Current?.DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
            return;
        }
        if (sender is CommentCardView commentCard)
            if (commentCard.IsLike)
            {
                try
                {
                    UserInfoProfile.CurrentUser.LikedCommentID += new string[] { commentCard.CommentID }.ToXFEString();
                    await UserInfoPage.UpLoadUserInfo();
                    if (await commentCard.CurrentCommentData.ExecuteUpdate(XFEExecuter) == 0)
                    {
                        commentCard.IsLike = false;
                        await Shell.Current?.DisplayAlert("点赞失败", "请检查网络设置", "确定");
                    }
                }
                catch (Exception ex)
                {
                    commentCard.IsLike = false;
                    await Shell.Current?.DisplayAlert("点赞失败", "请检查网络设置" + ex.Message, "确定");
                }
            }
            else
            {
                try
                {
                    UserInfoProfile.CurrentUser.LikedCommentID = UserInfoProfile.CurrentUser.LikedCommentID.Replace($"[+-{commentCard.CommentID}-+]", string.Empty);
                    await UserInfoPage.UpLoadUserInfo();
                    if (await commentCard.CurrentCommentData.ExecuteUpdate(XFEExecuter) == 0)
                    {
                        commentCard.IsLike = true;
                        await Shell.Current?.DisplayAlert("取消点赞失败", "请检查网络设置", "确定");
                    }
                }
                catch (Exception ex)
                {
                    commentCard.IsLike = true;
                    await Shell.Current?.DisplayAlert("取消点赞失败", "请检查网络设置" + ex.Message, "确定");
                }
            }
    }

    private async void TagButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current?.DisplayAlert("标签", $"{(sender as Button).Text[1..]}", "确定");
    }

    internal void InputEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewPage.inputEditor.Text))
        {
            ViewPage.sendButton.BackgroundColor = Color.FromArgb("#A491E8");
            ViewPage.sendButton.IsEnabled = false;
        }
        else
        {
            ViewPage.sendButton.BackgroundColor = Color.FromArgb("#512BD4");
            ViewPage.sendButton.IsEnabled = true;
        }
    }

    internal async void LikeButton_Clicked(object sender, EventArgs e)
    {
        if (!UserInfoProfile.LoginSuccessful)
        {
            if (await Shell.Current?.DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
            return;
        }
        if (IsLike)
        {
            try
            {
                CurrentPostData.PostLike++;
                LikeCount++;
                UserInfoProfile.CurrentUser.LikedPostID += new string[] { CurrentPostData.PostID }.ToXFEString();
                await UserInfoPage.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostLike--;
                    IsLike = false;
                    await Shell.Current?.DisplayAlert("点赞失败", "请检查网络设置", "确定");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData.PostLike--;
                IsLike = false;
                await Shell.Current?.DisplayAlert("点赞失败", "请检查网络设置" + ex.Message, "确定");
            }
        }
        else
        {
            try
            {
                if (CurrentPostData.PostLike >= 0)
                {
                    CurrentPostData.PostLike--;
                    LikeCount--;
                }
                UserInfoProfile.CurrentUser.LikedPostID = UserInfoProfile.CurrentUser.LikedPostID.Replace($"[+-{CurrentPostData.PostID}-+]", string.Empty);
                await UserInfoPage.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostLike++;
                    LikeCount++;
                    IsLike = true;
                    await Shell.Current?.DisplayAlert("取消点赞失败", "请检查网络设置", "确定");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData.PostLike++;
                LikeCount++;
                IsLike = true;
                await Shell.Current?.DisplayAlert("取消点赞失败", "请检查网络设置" + ex.Message, "确定");
            }
        }
    }

    internal async void StarButton_Clicked(object sender, EventArgs e)
    {
        if (!UserInfoProfile.LoginSuccessful)
        {
            if (await Shell.Current?.DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
            return;
        }
        if (IsStar)
        {
            try
            {
                CurrentPostData.PostStar++;
                StarCount++;
                UserInfoProfile.CurrentUser.StarredPostID += new string[] { CurrentPostData.PostID }.ToXFEString();
                await UserInfoPage.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostStar--;
                    IsStar = false;
                    await Shell.Current?.DisplayAlert("收藏失败", "请检查网络设置", "确定");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData.PostStar--;
                IsStar = false;
                await Shell.Current?.DisplayAlert("收藏失败", "请检查网络设置" + ex.Message, "确定");
            }
        }
        else
        {
            try
            {
                if (CurrentPostData.PostStar >= 0)
                {
                    CurrentPostData.PostStar--;
                    StarCount--;
                }
                UserInfoProfile.CurrentUser.StarredPostID = UserInfoProfile.CurrentUser.StarredPostID.Replace($"[+-{CurrentPostData.PostID}-+]", string.Empty);
                await UserInfoPage.UpLoadUserInfo();
                if (await CurrentPostData.ExecuteUpdate(XFEExecuter) == 0)
                {
                    CurrentPostData.PostStar++;
                    StarCount++;
                    IsStar = true;
                    await Shell.Current?.DisplayAlert("取消收藏失败", "请检查网络设置", "确定");
                }
            }
            catch (Exception ex)
            {
                CurrentPostData.PostStar++;
                StarCount++;
                IsStar = true;
                await Shell.Current?.DisplayAlert("取消收藏失败", "请检查网络设置" + ex.Message, "确定");
            }
        }
    }

    [RelayCommand]
    private async Task SendClick()
    {
        try
        {
            if (!UserInfoProfile.LoginSuccessful)
            {
                if (await Shell.Current?.DisplayAlert("诶嘿", "请先登录哦", "前往登录", "取消"))
                {
                    await Shell.Current.GoToAsync(nameof(UserLoginPage));
                }
                return;
            }
            ViewPage.sendButton.IsEnabled = false;
            ViewPage.sendButton.BackgroundColor = Color.FromArgb("#A491E8");
            ViewPage.inputEditor.IsEnabled = false;
            var tarCommentId = await IDGenerator.GetCorrectCommentId(XFEExecuter);
            try
            {
                var result = await XFEExecuter.ExecuteAdd(new XFEChatRoom_CommunityComment
                {
                    CommentID = tarCommentId,
                    PostID = ViewPage.PostID,
                    CommentContent = InputText,
                    UID = UserInfoProfile.CurrentUser.ID,
                    UName = UserInfoProfile.CurrentUser.Aname,
                    FloorCount = await AppAlgorithm.GetCommentFloor(ViewPage.PostID, XFEExecuter),
                    QuoteID = quoteID
                });
                if (result == 0)
                {
                    ViewPage.sendButton.BackgroundColor = Color.FromArgb("#512BD4");
                    ViewPage.inputEditor.IsEnabled = true;
                    ViewPage.sendButton.IsEnabled = true;
                    await PopupAction.DisplayPopup(new ErrorPopup("评论失败", "请检查网络设置"));
                    return;
                }
            }
            catch (Exception) { }
            XFEExecuter.RefreshExecuter();
            LoadComment();
            ViewPage.inputEditor.IsEnabled = true;
            ViewPage.inputEditor.Text = string.Empty;
            CloseQuote();
            await ViewPage.commentScrollView.ScrollToAsync(ViewPage.commentStack, ScrollToPosition.End, false);
            await PopupAction.DisplayPopup(new TipPopup("评论成功", 1));
        }
        catch (Exception ex)
        {
            ViewPage.sendButton.BackgroundColor = Color.FromArgb("#512BD4");
            ViewPage.inputEditor.IsEnabled = true;
            ViewPage.sendButton.IsEnabled = true;
            await PopupAction.DisplayPopup(new ErrorPopup("评论失败", $"请检查网络设置\n{ex.Message}"));
            Trace.WriteLine(ex.ToString());
            return;
        }
    }

    [RelayCommand]
    private void QuoteClose()
    {
        CloseQuote();
    }
}
