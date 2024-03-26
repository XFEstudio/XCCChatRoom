using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPopup;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.ArrayExtension;
using XFEExtension.NetCore.ListExtension;
using XFEExtension.NetCore.TaskExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewModel;

internal partial class PostEditPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string titleText;
    [ObservableProperty]
    private string contentText;
    public PostEditPage ViewPage { get; init; }
    public XFEChatRoom_CommunityPost CurrentPostData { get; set; }

    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    private readonly List<string> tags = [];
    internal bool backTrigger = false;
    internal bool secTrigger = false;
    private bool posting = false;
    private bool deleting = false;
    public PostEditPageViewModel(PostEditPage viewModel)
    {
        ViewPage = viewModel;

        CurrentPostData = PostViewPage.Current?.ViewModel.CurrentPostData;
        if (CurrentPostData is not null)
        {
            TitleText = CurrentPostData.PostTitle;
            ContentText = CurrentPostData.PostContent;
            foreach (var tag in CurrentPostData.PostTag.ToXFEArray<string>())
            {
                if (tags.Contains(tag))
                    continue;
                tags.Add(tag);
                var button = new Button
                {
                    Text = $"#{tag}",
                    BackgroundColor = Color.FromArgb("#F0ECFE"),
                    TextColor = Color.FromArgb("#512BD4"),
                    CornerRadius = 30,
                    Margin = new Thickness(10, 0, 0, 0)
                };
                button.Clicked += TagButton_Clicked;
                ViewPage.tagStackLayout.Children.Add(button);
            }
        }
    }

    internal void ContentEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ContentText.Length > 3000)
        {
            ViewPage.wordCountLabel.TextColor = Color.Parse("Red");
            ViewPage.wordCountLabel.Opacity = 1;
        }
        else
        {
            ViewPage.wordCountLabel.TextColor = Color.Parse("Gray");
            ViewPage.wordCountLabel.Opacity = 0.5;
        }
    }

    private void TagButton_Clicked(object sender, EventArgs e)
    {
        ViewPage.tagStackLayout.Children.Remove(sender as Button);
    }

    [RelayCommand]
    async Task DeletePost()
    {
        if (deleting)
        {
            return;
        }
        deleting = true;
        backTrigger = true;
        if (await Shell.Current?.DisplayAlert("删除帖子", "确认删除吗？删除后的帖子不可恢复", "确认", "取消"))
            try
            {
                var task = PopupAction.DisplayPopup(new TipPopup("删除中...", 300));
                var tarPost = await XFEExecuter.ExecuteGetFirst<XFEChatRoom_CommunityPost>(x => x.PostID == CurrentPostData.PostID);
                var result = await XFEExecuter.ExecuteDelete(tarPost);
                if (result == 0)
                {
                    await PopupAction.ClosePopup();
                    backTrigger = false;
                    deleting = false;
                    await PopupAction.DisplayPopup(new ErrorPopup("删除失败", "请检查网络设置并尝试重新发布"));
                    return;
                }
                await Task.Delay(800);
                await PopupAction.ClosePopup();
                await ViewPage.Content.FadeTo(0, 300, Easing.CubicInOut);
                var successfulLabel = new Label
                {
                    Text = "删除成功",
                    Opacity = 0,
                    TextColor = Color.FromArgb("#512BD4"),
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 40,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                ViewPage.Content = successfulLabel;
                await successfulLabel.FadeTo(1, 300, Easing.CubicInOut);
                await Task.Delay(1000);
                await successfulLabel.FadeTo(0, 300, Easing.CubicInOut);
                PostViewPage.Current.ViewModel.CurrentPostData = null;
                await Shell.Current.GoToAsync("../..");
                CommunityPage.Current.ViewModel.RemovePostByID(CurrentPostData?.PostID);
                await CommunityPage.Current.ViewModel.PostRefresh();
                backTrigger = false;
                deleting = false;
            }
            catch (Exception ex)
            {
                await PopupAction.ClosePopup();
                backTrigger = false;
                deleting = false;
                await PopupAction.DisplayPopup(new ErrorPopup("删除失败", $"请检查网络设置并尝试重新发布{ex.Message}"));
                return;
            }
        else
            deleting = false;
    }

    [RelayCommand]
    async Task PublishPost()
    {
        if (posting)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(TitleText))
        {
            await Shell.Current?.DisplayAlert("啊哦！QAQ", "标题不能为空", "确定");
            return;
        }
        if (string.IsNullOrWhiteSpace(ContentText))
        {
            await Shell.Current?.DisplayAlert("啊哦！QAQ", "内容不能为空", "确定");
            return;
        }
        if (ContentText.Length > 3000)
        {
            await Shell.Current?.DisplayAlert("啊哦！QAQ", "内容字数过长\n可在评论区继续未完成的创作哦~", "确定");
            return;
        }
        backTrigger = true;
        posting = true;
        var task = PopupAction.DisplayPopup(new TipPopup("发布中...", 300));
        var timeSpend = new Action(async () =>
        {
            if (CurrentPostData is null)
            {
                try
                {
                    var result = await XFEExecuter.ExecuteAdd(new XFEChatRoom_CommunityPost
                    {
                        PostTitle = TitleText,
                        PostContent = ContentText,
                        UName = UserInfoProfile.Name,
                        UID = UserInfoProfile.UUID,
                        PostID = await IDGenerator.GetCorrectPostID(XFEExecuter),
                        PostTag = tags.ToXFEString()
                    });
                    if (result == 0)
                    {
                        await PopupAction.ClosePopup();
                        backTrigger = false;
                        posting = false;
                        await PopupAction.DisplayPopup(new ErrorPopup("发布失败", "请检查网络设置并尝试重新发布"));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    await PopupAction.ClosePopup();
                    backTrigger = false;
                    posting = false;
                    await PopupAction.DisplayPopup(new ErrorPopup("发布失败", $"请检查网络设置并尝试重新发布{ex.Message}"));
                    return;
                }
            }
            else
            {
                CurrentPostData = await XFEExecuter.ExecuteGetFirst<XFEChatRoom_CommunityPost>(x => x.PostID == CurrentPostData.PostID);
                CurrentPostData.PostTitle = TitleText;
                CurrentPostData.PostContent = ContentText;
                CurrentPostData.PostTag = tags.ToXFEString();
                try
                {
                    var result = await XFEExecuter.ExecuteUpdate(CurrentPostData);
                    if (result == 0)
                    {
                        await PopupAction.ClosePopup();
                        backTrigger = false;
                        posting = false;
                        await PopupAction.DisplayPopup(new ErrorPopup("发布编辑失败", "请检查网络设置并尝试重新发布"));
                        return;
                    }
                    await PostViewPage.Current.ViewModel.Refresh();
                }
                catch (Exception ex)
                {
                    await PopupAction.ClosePopup();
                    backTrigger = false;
                    posting = false;
                    await PopupAction.DisplayPopup(new ErrorPopup("发布失败", $"请检查网络设置并尝试重新发布{ex.Message}"));
                    return;
                }
            }
        }).CTime(true, "发布花费").TotalSeconds;
        if (timeSpend < 1)
            await Task.Delay(800);
        await PopupAction.ClosePopup();
        await ViewPage.Content.FadeTo(0, 300, Easing.CubicInOut);
        var successfulLabel = new Label
        {
            Text = "发布成功",
            Opacity = 0,
            TextColor = Color.FromArgb("#512BD4"),
            FontAttributes = FontAttributes.Bold,
            FontSize = 40,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        ViewPage.Content = successfulLabel;
        await successfulLabel.FadeTo(1, 300, Easing.CubicInOut);
        await Task.Delay(1000);
        await successfulLabel.FadeTo(0, 300, Easing.CubicInOut);
        CommunityPage.Current?.ViewModel.PostRefresh();
        if (CurrentPostData is not null)
        {
            PostViewPage.Current?.ViewModel.Refresh();
        }
        Shell.Current.SendBackButtonPressed();
        posting = false;
    }

    [RelayCommand]
    async Task AddTag()
    {
        var result = await Shell.Current?.DisplayPromptAsync("添加标签", "请输入标签名称", "确定", "取消", "不能为空或含有空格", 10, null);
        if (result is null)
            return;
        if (string.IsNullOrWhiteSpace(result) || result.Contains(' '))
        {
            await Shell.Current?.DisplayAlert("哦不QAQ", "标签名称不能为空或含有空格", "确定");
            return;
        }
        if (tags.Contains(result))
        {
            await Shell.Current?.DisplayAlert("哦不QAQ", "标签不能重复", "好吧");
            return;
        }
        tags.Add(result);
        var button = new Button
        {
            Text = $"#{result}",
            BackgroundColor = Color.FromArgb("#F0ECFE"),
            TextColor = Color.FromArgb("#512BD4"),
            CornerRadius = 30,
            Margin = new Thickness(10, 0, 0, 0)
        };
        button.Clicked += TagButton_Clicked;
        ViewPage.tagStackLayout.Children.Add(button);
    }
}
