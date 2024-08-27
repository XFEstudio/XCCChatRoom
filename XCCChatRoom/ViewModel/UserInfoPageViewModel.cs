using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XCCChatRoom.Controls;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.ProfileExtension;

namespace XCCChatRoom.ViewModel;

internal partial class UserInfoPageViewModel : ObservableObject
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
        SwitchToTheme(SystemProfile.Theme);
        if (UserInfoProfile.LoginSuccessful)
        {
            UserName = UserInfoProfile.Name;
            UserNameFirstLatter = UserInfoProfile.Name[0].ToString();
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
        ViewPage.loginButton.SetDynamicResource(Button.BackgroundColorProperty, "MainColor");
        ViewPage.loginButton.Text = "登录";
        ViewPage.loginButton.BorderColor = null;
        ViewPage.loginButton.BorderWidth = 0;
        ViewPage.loginButton.TextColor = Color.Parse("White");
        GroupContactPage.Current.ViewModel.RemoveOtherGroup();
        GroupContactPage.Current.ViewModel.UserName = string.Empty;
        (GroupContactPage.Current?.groupStackLayout.Children.First() as GroupCardView).IsVisible = false;
        CommunityPage.Current?.ViewModel.ChangeToUnLoginStyle();
        try
        {
            XFEProfile.DeleteProfile(typeof(UserInfoProfile));
        }
        catch (Exception ex)
        {
            await Shell.Current!.DisplayAlert("错误", ex.Message, "确认");
        }
    }

    [RelayCommand]
    private void SwitchToNextTheme()
    {
        switch (SystemProfile.Theme)
        {
            case AppTheme.Unspecified:
                SwitchToTheme(AppTheme.Light);
                SystemProfile.Theme = AppTheme.Light;
                Application.Current!.UserAppTheme = AppTheme.Light;
                SwitchToTheme(AppTheme.Light);
                break;
            case AppTheme.Light:
                SwitchToTheme(AppTheme.Dark);
                SystemProfile.Theme = AppTheme.Dark;
                Application.Current!.UserAppTheme = AppTheme.Dark;
                SwitchToTheme(AppTheme.Dark);
                App.AutoSwitchByTheme(Application.Current!.RequestedTheme);
                break;
            case AppTheme.Dark:
                SwitchToTheme(AppTheme.Unspecified);
                SystemProfile.Theme = AppTheme.Unspecified;
                Application.Current!.UserAppTheme = AppTheme.Unspecified;
                SwitchToTheme(AppTheme.Unspecified);
                App.AutoSwitchByTheme(Application.Current!.RequestedTheme);
                break;
            default:
                break;
        }
    }

    public void SwitchToTheme(AppTheme appTheme)
    {
        switch (appTheme)
        {
            case AppTheme.Unspecified:
                ViewPage.switchColorButton.IconImageSource = "sunandmoon.png";
                ViewPage.switchColorButton.Text = "跟随系统";
                break;
            case AppTheme.Light:
                ViewPage.switchColorButton.IconImageSource = "sun.png";
                ViewPage.switchColorButton.Text = "浅色主题";
                break;
            case AppTheme.Dark:
                ViewPage.switchColorButton.IconImageSource = "moon.png";
                ViewPage.switchColorButton.Text = "深色主题";
                break;
            default:
                break;
        }
    }
}
