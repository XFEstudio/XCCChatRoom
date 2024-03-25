using CommunityToolkit.Mvvm.ComponentModel;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom.ViewModel;

public partial class UserInfoPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string userName;
    [ObservableProperty]
    private string userNameFirstLatter;
    [ObservableProperty]
    private string uUID;

    public UserInfoPage ViewPage { get; set; }

    public UserInfoPageViewModel(UserInfoPage viewPage)
    {
        ViewPage = viewPage;
        if (UserInfoProfile.LoginSuccessful)
        {
            UserName = UserInfoProfile.Name;
            UUID = UserInfoProfile.UUID;
            SwitchToLoginStyle();
        }
        else
        {
            UserNameFirstLatter = "?";
            UserName = "未登录";
            UUID = "暂无UID";
            UserInfoProfile.Name = string.Empty;
            UserInfoProfile.UUID = string.Empty;
            UserInfoProfile.Password = string.Empty;
            UserInfoProfile.Phone = string.Empty;
        }
    }

    public void SwitchToLoginStyle()
    {
        ViewPage.loginButton.BackgroundColor = Color.Parse("White");
        ViewPage.loginButton.Text = "退出登录";
        ViewPage.loginButton.BorderColor = Color.Parse("Red");
        ViewPage.loginButton.BorderWidth = 1;
        ViewPage.loginButton.TextColor = Color.Parse("Red");
        ViewPage.loginButton.WaitClick -= ViewPage.LoginButton_WaitClick;
        ViewPage.loginButton.WaitClick += ViewPage.UnLoginButton_WaitClick;
    }

    public async Task SwitchToUnLoginStyle()
    {
        UserInfoProfile.LoginSuccessful = false;
        UserNameFirstLatter = "?";
        UserName = "未登录";
        UUID = "暂无UID";
        UserInfoProfile.Name = string.Empty;
        UserInfoProfile.UUID = string.Empty;
        UserInfoProfile.Password = string.Empty;
        UserInfoProfile.Phone = string.Empty;
        ViewPage.loginButton.BackgroundColor = Color.FromArgb("#512BD4");
        ViewPage.loginButton.Text = "登录";
        ViewPage.loginButton.BorderColor = null;
        ViewPage.loginButton.BorderWidth = 0;
        ViewPage.loginButton.TextColor = Color.Parse("White");
        GroupContactPage.Current.RemoveOtherGroup();
        GroupContactPage.Current.UserName = string.Empty;
        (GroupContactPage.Current.GroupStackLayout.Children.First() as GroupCardView).IsVisible = false;
        CommunityPage.Current?.ChangeToUnLoginStyle();
        try
        {
            File.Delete(AppPath.ProfilesPath);
        }
        catch (Exception ex)
        {
            await Shell.Current?.DisplayAlert("错误", ex.Message, "确认");
        }
    }
}
