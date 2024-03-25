using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TencentCloud.Ame.V20190916.Models;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.FormatExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewModel;

public partial class GroupContactPageViewModel : ObservableObject
{
    public GroupContactPage ViewPage { get; init; }
    private string userName;
    public string UserName
    {
        get => userName;
        set
        {
            userName = value;
            ViewPage.officalGroupCardView.UserNameInGroup = value;
        }
    }
    private XFEDictionary groups = [];
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();


    public GroupContactPageViewModel(GroupContactPage viewPage)
    {
        ViewPage = viewPage;
        ViewPage.Loaded += ViewPageLoaded;
        UserName = UserInfoProfile.Name;
        ViewPage.officalGroupCardView.Click += GroupCardView_Click;
        ViewPage.officalGroupCardView.Swipe += GroupCardView_Swipe;
    }

    private async void ViewPageLoaded(object sender, EventArgs e)
    {
        AppUpdate.StartCheckUpdate(ViewPage);
        await UserInfoPage.ReadUserData(ViewPage);
        ViewPage.groupRefreshView.IsRefreshing = true;
    }

    private async void GroupCardView_Swipe(object sender, GroupCardViewSwipeEventArgs e)
    {
        if (sender is GroupCardView groupCardView)
        {
            await groupCardView.TranslateTo(-200, 0, 80, Easing.SpringOut);
            if (groupCardView.GroupName == "XFE聊天室[官方]")
            {
                await groupCardView.TranslateTo(0, 0, 80, Easing.SpringOut);
                return;
            }
            var result = await Shell.Current?.DisplayAlert("删除", "是否删除该群组？", "是", "否");
            if (result)
            {
                ViewPage.groupStackLayout.Children.Remove(groupCardView);
                groups.Remove(groupCardView.GroupName);
                await UpLoadGroup();
            }
            else
            {
                await groupCardView.TranslateTo(0, 0, 80, Easing.SpringOut);
            }
        }
    }

    private async void GroupCardView_Click(object sender, GroupCardViewClickEventArgs e) => await Shell.Current.GoToAsync($"{nameof(ChatPage)}?GroupName={e.GroupName}&CurrentName={e.UserNameInGroup}");

    public void RemoveOtherGroup()
    {
        for (int i = ViewPage.groupStackLayout.Children.Count; i > 1; i--)
        {
            ViewPage.groupStackLayout.Children.RemoveAt(i - 1);
        }
    }

    private async Task UpLoadGroup()
    {
        try
        {
            UserInfoProfile.CurrentUser.Agroups = groups.ToString();
            await UserInfoProfile.CurrentUser.ExecuteUpdate(XFEExecuter);
        }
        catch (Exception ex)
        {
            await Shell.Current?.DisplayAlert("错误", ex.Message, "确定");
        }
    }

    private void LoadGroup()
    {
        if (UserInfoProfile.LoginSuccessful)
        {
            ViewPage.officalGroupCardView.IsVisible = true;
            try
            {
                groups = new XFEDictionary(UserInfoProfile.CurrentUser.Agroups);
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
                        ViewPage.groupStackLayout.Children.Add(groupView);
                    }
                }
            }
            catch (Exception ex)
            {
                Shell.Current?.DisplayAlert("错误", ex.Message, "确定");
            }
        }
        else
        {
            ViewPage.officalGroupCardView.IsVisible = false;
        }
    }

    [RelayCommand]
    async Task AddGroup()
    {
        if (UserInfoProfile.LoginSuccessful)
        {
            var targetGroupName = await Shell.Current?.DisplayPromptAsync("添加群组", "请输入群组名称", "确定", "取消", "群组名称", 20, Keyboard.Default, string.Empty);
            if (string.IsNullOrWhiteSpace(targetGroupName))
            {
                if (targetGroupName == string.Empty)
                    await Shell.Current?.DisplayAlert("错误", "群组名称不能为空", "确定");
                return;
            }
            else
            {
                var userNameInGroup = await Shell.Current?.DisplayPromptAsync("群内身份", "请输入在该群中的名称", "确定", null, "群内名称", 20, Keyboard.Default, UserInfoProfile.Name);
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
                    ViewPage.groupStackLayout.Children.Add(groupView);
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
    }

    [RelayCommand]
    void Refresh()
    {
        RemoveOtherGroup();
        LoadGroup();
        ViewPage.groupRefreshView.IsRefreshing = false;
    }
}
