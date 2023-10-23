using MauiPopup;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XFE各类拓展.ArrayExtension;
using XFE各类拓展.ListExtension;
using XFE各类拓展.NetCore.XFEDataBase;
using XFE各类拓展.TaskExtension;

namespace XCCChatRoom.InnerPage;

public partial class PostEditPage : ContentPage
{
    public XFEChatRoom_CommunityPost CurrentPostData { get; set; }
    private XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    private List<string> tags = new List<string>();
    private bool BackTrigger = false;
    private bool SecTrigger = false;
    private bool Posting = false;
    private bool Deleting = false;
    public PostEditPage()
    {
        InitializeComponent();
        CurrentPostData = PostViewPage.Current?.CurrentPostData;
        if (CurrentPostData is not null)
        {
            TitleEditor.Text = CurrentPostData.PostTitle;
            ContentEditor.Text = CurrentPostData.PostContent;
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
                TagStackLayout.Children.Add(button);
            }
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "删除",
                IconImageSource = "trash",
                Command = new Command(async () =>
                {
                    if (Deleting)
                    {
                        return;
                    }
                    Deleting = true;
                    BackTrigger = true;
                    if (await DisplayAlert("删除帖子", "确认删除吗？删除后的帖子不可恢复", "确认", "取消"))
                        try
                        {
                            var task = PopupAction.DisplayPopup(new LoadingPopup("删除中...", 300));
                            var tarPost = await XFEExecuter.ExecuteGetFirst<XFEChatRoom_CommunityPost>(x => x.PostID == CurrentPostData.PostID);
                            var result = await XFEExecuter.ExecuteDelete(tarPost);
                            if (result == 0)
                            {
                                await PopupAction.ClosePopup();
                                BackTrigger = false;
                                Deleting = false;
                                await PopupAction.DisplayPopup(new ErrorPopup("删除失败", "请检查网络设置并尝试重新发布"));
                                return;
                            }
                            await Task.Delay(800);
                            await PopupAction.ClosePopup();
                            await this.Content.FadeTo(0, 300, Easing.CubicInOut);
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
                            this.Content = successfulLabel;
                            await successfulLabel.FadeTo(1, 300, Easing.CubicInOut);
                            await Task.Delay(1000);
                            await successfulLabel.FadeTo(0, 300, Easing.CubicInOut);
                            PostViewPage.Current.CurrentPostData = null;
                            await Shell.Current.GoToAsync("../..");
                            CommunityPage.Current.RemovePostByID(CurrentPostData?.PostID);
                            CommunityPage.Current.postRefreshView_Refreshing(null, null);
                            BackTrigger = false;
                            Deleting = false;
                        }
                        catch (Exception ex)
                        {
                            await PopupAction.ClosePopup();
                            BackTrigger = false;
                            Deleting = false;
                            await PopupAction.DisplayPopup(new ErrorPopup("删除失败", $"请检查网络设置并尝试重新发布{ex.Message}"));
                            return;
                        }
                    else
                        Deleting = false;
                })
            });
        }
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "发布",
            Command = new Command(async () =>
            {
                if (Posting)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(TitleEditor.Text))
                {
                    await DisplayAlert("啊哦！QAQ", "标题不能为空", "确定");
                    return;
                }
                if (string.IsNullOrWhiteSpace(ContentEditor.Text))
                {
                    await DisplayAlert("啊哦！QAQ", "内容不能为空", "确定");
                    return;
                }
                if (ContentEditor.Text.Length > 3000)
                {
                    await DisplayAlert("啊哦！QAQ", "内容字数过长\n可在评论区继续未完成的创作哦~", "确定");
                    return;
                }
                BackTrigger = true;
                Posting = true;
                var task = PopupAction.DisplayPopup(new LoadingPopup("发布中...", 300));
                var timeSpend = new Action(async () =>
                {
                    if (CurrentPostData is null)
                    {
                        try
                        {
                            var result = await XFEExecuter.ExecuteAdd(new XFEChatRoom_CommunityPost
                            {
                                PostTitle = TitleEditor.Text,
                                PostContent = ContentEditor.Text,
                                UName = UserInfo.StaticUserName,
                                UID = UserInfo.StaticUUID,
                                PostID = await IDGenerator.GetCorrectPostID(XFEExecuter),
                                PostTag = tags.ToXFEString()
                            });
                            if (result == 0)
                            {
                                await PopupAction.ClosePopup();
                                BackTrigger = false;
                                Posting = false;
                                await PopupAction.DisplayPopup(new ErrorPopup("发布失败", "请检查网络设置并尝试重新发布"));
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            await PopupAction.ClosePopup();
                            BackTrigger = false;
                            Posting = false;
                            await PopupAction.DisplayPopup(new ErrorPopup("发布失败", $"请检查网络设置并尝试重新发布{ex.Message}"));
                            return;
                        }
                    }
                    else
                    {
                        CurrentPostData = await XFEExecuter.ExecuteGetFirst<XFEChatRoom_CommunityPost>(x => x.PostID == CurrentPostData.PostID);
                        CurrentPostData.PostTitle = TitleEditor.Text;
                        CurrentPostData.PostContent = ContentEditor.Text;
                        CurrentPostData.PostTag = tags.ToXFEString();
                        try
                        {
                            var result = await XFEExecuter.ExecuteUpdate(CurrentPostData);
                            if (result == 0)
                            {
                                await PopupAction.ClosePopup();
                                BackTrigger = false;
                                Posting = false;
                                await PopupAction.DisplayPopup(new ErrorPopup("发布编辑失败", "请检查网络设置并尝试重新发布"));
                                return;
                            }
                            await PostViewPage.Current.Refresh();
                        }
                        catch (Exception ex)
                        {
                            await PopupAction.ClosePopup();
                            BackTrigger = false;
                            Posting = false;
                            await PopupAction.DisplayPopup(new ErrorPopup("发布失败", $"请检查网络设置并尝试重新发布{ex.Message}"));
                            return;
                        }
                    }
                }).CTime(true, "发布花费").TotalSeconds;
                if (timeSpend < 1)
                    await Task.Delay(800);
                await PopupAction.ClosePopup();
                await this.Content.FadeTo(0, 300, Easing.CubicInOut);
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
                this.Content = successfulLabel;
                await successfulLabel.FadeTo(1, 300, Easing.CubicInOut);
                await Task.Delay(1000);
                await successfulLabel.FadeTo(0, 300, Easing.CubicInOut);
                CommunityPage.Current.postRefreshView_Refreshing(null, null);
                if (CurrentPostData is not null)
                {
                    PostViewPage.Current?.Refresh();
                }
                Shell.Current.SendBackButtonPressed();
                Posting = false;
            })
        });
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (TitleEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (ContentEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }
    protected override bool OnBackButtonPressed()
    {
        if (!BackTrigger && !SecTrigger)
        {
            this.Dispatcher.Dispatch(async () =>
            {
                if (await DisplayAlert("确定退出吗？", "未保存的内容将会丢失，请谨慎选择", "退出", "取消"))
                {
                    SecTrigger = true;
                    Shell.Current.SendBackButtonPressed();
                }
                else
                {
                    SecTrigger = false;
                }
            });
            return true;
        }
        else
        {
            return base.OnBackButtonPressed();
        }
    }

    private void ContentEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ContentEditor.Text.Length > 3000)
        {
            WordCountLabel.TextColor = Color.Parse("Red");
            WordCountLabel.Opacity = 1;
        }
        else
        {
            WordCountLabel.TextColor = Color.Parse("Gray");
            WordCountLabel.Opacity = 0.5;
        }
        WordCountLabel.Text = $"字数：{ContentEditor.Text.Length}/3000";
    }

    private async void AddTagButton_Clicked(object sender, EventArgs e)
    {
        var str = await DisplayPromptAsync("添加标签", "请输入标签名称", "确定", "取消", "不能为空或含有空格", 10, null);
        if (str is null)
            return;
        if (string.IsNullOrWhiteSpace(str) || str.Contains(' '))
        {
            await DisplayAlert("哦不QAQ", "标签名称不能为空或含有空格", "确定");
            return;
        }
        if (tags.Contains(str))
        {
            await DisplayAlert("哦不QAQ", "标签不能重复", "好吧");
            return;
        }
        tags.Add(str);
        var button = new Button
        {
            Text = $"#{str}",
            BackgroundColor = Color.FromArgb("#F0ECFE"),
            TextColor = Color.FromArgb("#512BD4"),
            CornerRadius = 30,
            Margin = new Thickness(10, 0, 0, 0)
        };
        button.Clicked += TagButton_Clicked;
        TagStackLayout.Children.Add(button);
    }
    private void TagButton_Clicked(object sender, EventArgs e)
    {
        TagStackLayout.Children.Remove(sender as Button);
    }
}