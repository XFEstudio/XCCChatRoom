using MauiPopup;
using System.Diagnostics;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Model;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewModel;
using XFEExtension.NetCore.FileExtension;
using XFEExtension.NetCore.FormatExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewPage;
public partial class UserInfoPage : ContentPage
{
    public UserInfoPageViewModel ViewModel { get; set; }
    public static UserInfoPage Current { get; private set; }
    private static XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    public UserInfoPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
        Current = this;
    }

    public async static Task<int> UpLoadUserInfo()
    {
        try
        {
            return await UserInfoProfile.CurrentUser.ExecuteUpdate(XFEExecuter);
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.ToString());
            return 0;
        }
    }

    public void SwitchToLoginStyle()
    {
        ViewModel.SwitchToLoginStyle();
    }

    public async Task SwitchToUnLoginStyle()
    {
        await ViewModel.SwitchToUnLoginStyle();
    }

    public static void EditUserProperty(UserPropertyToEdit userPropertyToEdit, string newProperty, Page page)
    {
        switch (userPropertyToEdit)
        {
            case UserPropertyToEdit.UserName:
                UserInfoProfile.Name = newProperty;
                Current.ViewModel.UserName = newProperty;
                CurrentUser.Aname = newProperty;
                break;

            case UserPropertyToEdit.Password:
                StaticPassword = newProperty;
                CurrentUser.Apassword = newProperty;
                break;

            case UserPropertyToEdit.PhoneNum:
                StaticMail = newProperty;
                CurrentUser.Atel = newProperty;
                break;

            case UserPropertyToEdit.Mail:
                StaticMail = newProperty;
                CurrentUser.Amail = newProperty;
                break;
            default:
                ProcessException.ShowEnumException();
                break;
        }
        CurrentUser.ExecuteUpdate(XFEExecuter);
    }

    public static async Task ReadUserData(Page currentPage)
    {
        try
        {
            if (AppPath.UserInfoPath.ReadOut(out string userInfo))
            {
                if (userInfo is null)
                    return;
                var userProperties = new XFEDictionary(userInfo);
                if (userProperties is null || userProperties.Count == 0)
                {
                    SystemProfile.IsLoginSuccessful = false;
                }
                else
                {
                    foreach (var property in userProperties)
                    {
                        try
                        {
                            switch (property.Header)
                            {
                                case "UserName":
                                    StaticUserName = property.Content;
                                    break;
                                case "UUID":
                                    StaticUUID = property.Content;
                                    break;
                                case "Password":
                                    StaticPassword = property.Content;
                                    break;
                                case "PhoneNum":
                                    StaticPhoneNum = property.Content;
                                    break;
                                case "Mail":
                                    StaticMail = property.Content;
                                    break;
                                default:
                                    ProcessException.ShowEnumException();
                                    File.Delete(AppPath.UserInfoPath);
                                    return;
                            }
                        }
                        catch (Exception)
                        {
                            await currentPage.DisplayAlert("配置文件错误", "读取用户文件时发生错误\n用户配置文件损坏，请重新登录", "确认");
                            File.Delete(AppPath.UserInfoPath);
                            return;
                        }
                    }
                    var user = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == UserInfoProfile.PhoneNum);
                    if (user is null || user.Count == 0 || user.First() is null)
                    {
                        await currentPage.DisplayAlert("登录", "用户信息错误，请重新登录", "确认");
                        File.Delete(AppPath.UserInfoPath);
                        return;
                    }
                    else
                    {
                        if (user.First().Apassword != UserInfoProfile.Password)
                        {
                            await currentPage.DisplayAlert("登录", "用户密码错误\n密码可能已被修改，账号或存在风险\n请重新登录", "确认");
                            File.Delete(AppPath.UserInfoPath);
                            return;
                        }
                        else
                        {
                            CurrentUser = user.First();
                            UserInfoProfile.Email = CurrentUser.Amail;
                            UserInfoProfile.Name = CurrentUser.Aname;
                            UserInfoProfile.UUID = CurrentUser.ID;
                            UserInfoProfile.PhoneNum = CurrentUser.Atel;
                            UserInfoProfile.Password = CurrentUser.Apassword;
                            GroupContactPage.Current.UserName = CurrentUser.Aname;
                        }
                    }
                    UserInfoProfile.LoginSuccessful = true;
                }
            }
        }
        catch (Exception ex)
        {
            await currentPage.DisplayAlert("登录错误", ex.ToString(), "确认");
        }
    }

    internal async void LoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(UserLoginPage));
        e.Continue();
    }

    internal async void UnLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        if (await Shell.Current?.DisplayAlert("退出登录", "确定退出登录吗？", "确认", "取消"))
        {
            await SwitchToUnLoginStyle();
            loginButton.WaitClick -= UnLoginButton_WaitClick;
            loginButton.WaitClick += LoginButton_WaitClick;
        }
        e.Continue();
    }

    private void WhiteChoiceButton_Click(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync(nameof(UserPrivacyListPage));
    }

    private void WhiteChoiceUserPropertyEditorButton_Click(object sender, TappedEventArgs e)
    {
        if (UserInfoProfile.LoginSuccessful)
            Shell.Current.GoToAsync(nameof(UserPropertyEditPage));
        else
            PopupAction.DisplayPopup(new TipPopup("请先登录"));
    }
}