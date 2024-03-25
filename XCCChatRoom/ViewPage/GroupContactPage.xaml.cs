using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XFEExtension.NetCore.FormatExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewPage;

public partial class GroupContactPage : ContentPage
{
    public static GroupContactPage Current { get; set; }
    public string UserName
    {
        get => userName;
        set
        {
            userName = value;
            OfficalGroupCardView.UserNameInGroup = value;
        }
    }
    private XFEDictionary groups = new XFEDictionary();
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    private string userName;
    public GroupContactPage()
    {
        Current = this;
        InitializeComponent();
        this.Loaded += async (sender, e) =>
        {
            AppUpdate.StartCheckUpdate(this);
            await UserInfoPage.ReadUserData(this);
            groupRefreshView.IsRefreshing = true;
        };
        UserName = UserInfoPage.StaticUserName;
        //"测试".WriteIn($"{Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads)}/测试.txt");
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "添加群组",
            IconImageSource = "plus",
            Command = new Command(async () =>
            {
                if (UserInfoPage.IsLoginSuccessful)
                {
                    var targetGroupName = await DisplayPromptAsync("添加群组", "请输入群组名称", "确定", "取消", "群组名称", 20, Keyboard.Default, string.Empty);
                    if (string.IsNullOrWhiteSpace(targetGroupName))
                    {
                        if (targetGroupName == string.Empty)
                            await Shell.Current?.DisplayAlert("错误", "群组名称不能为空", "确定");
                        return;
                    }
                    else
                    {
                        var userNameInGroup = await DisplayPromptAsync("群内身份", "请输入在该群中的名称", "确定", null, "群内名称", 20, Keyboard.Default, UserInfoPage.StaticUserName);
                        if (string.IsNullOrWhiteSpace(userNameInGroup))
                        {
                            if (userNameInGroup == string.Empty)
                                await Shell.Current?.DisplayAlert("错误", "群内名称不能为空", "确定");
                            return;
                        }
                        try
                        {
                            groups.Add(targetGroupName, userNameInGroup);
                            var groupView = new GroupCardView
                            {
                                GroupName = targetGroupName,
                                UserNameInGroup = userNameInGroup,
                                Margin = new Thickness(0, 3, 0, 3)
                            };
                            groupView.Click += GroupCardView_Click;
                            groupView.Swipe += GroupCardView_Swipe;
                            GroupStackLayout.Children.Add(groupView);
                            await UpLoadGroup();
                        }
                        catch (Exception ex)
                        {
                            await Shell.Current?.DisplayAlert("群名重复", "群组名称不能重复\n原异常：" + ex.Message, "确定");
                        }
                    }
                }
                else
                {
                    await Shell.Current?.DisplayAlert("未登录", "请先登录", "确定");
                }
            })
        });
        OfficalGroupCardView.Click += GroupCardView_Click;
        OfficalGroupCardView.Swipe += GroupCardView_Swipe;
    }
    private void LoadGroup()
    {
        if (UserInfoPage.IsLoginSuccessful)
        {
            OfficalGroupCardView.IsVisible = true;
            try
            {
                groups = new XFEDictionary(UserInfoPage.CurrentUser.Agroups);
                if (groups.Count > 0)
                {
                    foreach (var item in groups)
                    {
                        var groupView = new GroupCardView
                        {
                            GroupName = item.Header,
                            UserNameInGroup = item.Content,
                            Margin = new Thickness(0, 3, 0, 3)
                        };
                        groupView.Click += GroupCardView_Click;
                        groupView.Swipe += GroupCardView_Swipe;
                        GroupStackLayout.Children.Add(groupView);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("错误", ex.Message, "确定");
            }
        }
        else
        {
            OfficalGroupCardView.IsVisible = false;
        }
    }
    private async Task UpLoadGroup()
    {
        try
        {
            UserInfoPage.CurrentUser.Agroups = groups.ToString();
            await UserInfoPage.CurrentUser.ExecuteUpdate(XFEExecuter);
        }
        catch (Exception ex)
        {
            await Shell.Current?.DisplayAlert("错误", ex.Message, "确定");
        }
    }
    private async void GroupCardView_Swipe(object sender, GroupCardViewSwipeEventArgs e)
    {
        var objSender = sender as GroupCardView;
        await objSender.TranslateTo(-200, 0, 80, Easing.SpringOut);
        if (objSender.GroupName == "XFE聊天室[官方]")
        {
            await objSender.TranslateTo(0, 0, 80, Easing.SpringOut);
            return;
        }
        var result = await Shell.Current?.DisplayAlert("删除", "是否删除该群组？", "是", "否");
        if (result)
        {
            GroupStackLayout.Children.Remove(objSender);
            groups.Remove(objSender.GroupName);
            await UpLoadGroup();
        }
        else
        {
            await objSender.TranslateTo(0, 0, 80, Easing.SpringOut);
        }
    }
    private async void GroupCardView_Click(object sender, GroupCardViewClickEventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(ChatPage)}?GroupName={e.GroupName}&CurrentName={e.UserNameInGroup}");
    }

    private void groupRefreshView_Refreshing(object sender, EventArgs e)
    {
        RemoveOtherGroup();
        LoadGroup();
        groupRefreshView.IsRefreshing = false;
    }
    public void RemoveOtherGroup()
    {
        for (int i = GroupStackLayout.Children.Count; i > 1; i--)
        {
            GroupStackLayout.Children.RemoveAt(i - 1);
        }
    }
}