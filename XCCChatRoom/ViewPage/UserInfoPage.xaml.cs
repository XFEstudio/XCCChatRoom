using MauiPopup;
using System.Diagnostics;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Model;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewModel;
using XFE������չ.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewPage;
public partial class UserInfoPage : ContentPage
{
    internal UserInfoPageViewModel ViewModel { get; set; }
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
                UserInfoProfile.CurrentUser.Aname = newProperty;
                break;

            case UserPropertyToEdit.Password:
                UserInfoProfile.Password = newProperty;
                UserInfoProfile.CurrentUser.Apassword = newProperty;
                break;

            case UserPropertyToEdit.PhoneNum:
                UserInfoProfile.Email = newProperty;
                UserInfoProfile.CurrentUser.Atel = newProperty;
                break;

            case UserPropertyToEdit.Mail:
                UserInfoProfile.Email = newProperty;
                UserInfoProfile.CurrentUser.Amail = newProperty;
                break;
            default:
                ProcessException.ShowEnumException();
                break;
        }
        UserInfoProfile.CurrentUser.ExecuteUpdate(XFEExecuter);
    }

    public static async Task ReadUserData(Page currentPage)
    {
        try
        {
            var user = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == UserInfoProfile.Phone);
            if (user is null || user.Count == 0 || user.First() is null)
            {
                await currentPage.DisplayAlert("��¼", "�û���Ϣ���������µ�¼", "ȷ��");
                File.Delete(AppPath.ProfilesPath);
                return;
            }
            else
            {
                if (user.First().Apassword != UserInfoProfile.Password)
                {
                    await currentPage.DisplayAlert("��¼", "�û��������\n��������ѱ��޸ģ��˺Ż���ڷ���\n�����µ�¼", "ȷ��");
                    File.Delete(AppPath.ProfilesPath);
                    return;
                }
                else
                {
                    UserInfoProfile.CurrentUser = user.First();
                    UserInfoProfile.Email = UserInfoProfile.CurrentUser.Amail;
                    UserInfoProfile.Name = UserInfoProfile.CurrentUser.Aname;
                    UserInfoProfile.UUID = UserInfoProfile.CurrentUser.ID;
                    UserInfoProfile.Phone = UserInfoProfile.CurrentUser.Atel;
                    UserInfoProfile.Password = UserInfoProfile.CurrentUser.Apassword;
                    GroupContactPage.Current.ViewModel.UserName = UserInfoProfile.CurrentUser.Aname;
                }
            }
        }
        catch (Exception ex)
        {
            await currentPage.DisplayAlert("��¼����", ex.ToString(), "ȷ��");
        }
    }

    internal async void LoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(UserLoginPage));
        e.Continue();
    }

    internal async void UnLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        if (await Shell.Current?.DisplayAlert("�˳���¼", "ȷ���˳���¼��", "ȷ��", "ȡ��"))
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
            PopupAction.DisplayPopup(new TipPopup("���ȵ�¼"));
    }
}