using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XCCChatRoom.Model;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.MailExtension;
using XFEExtension.NetCore.StringExtension;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.ViewModel;

internal partial class UserMailEditPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string primeMail;
    [ObservableProperty]
    private string newMail;
    [ObservableProperty]
    private string verifyCode;
    public UserMailEditPage ViewPage { get; init; }
    private bool flag1 = false;
    private bool flag2 = false;
    private bool flag3 = false; /*确认按钮允许点击的条件*/
    private string mailCaptcha = null;

    public UserMailEditPageViewModel(UserMailEditPage viewPage)
    {
        ViewPage = viewPage;
        PrimeMail = UserInfoProfile.CurrentUser.Amail;
    }
    internal async void NewMailEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (NewMail is not null)
        {
            if (NewMail.IsValidEmail()) { flag1 = true; }
            else
            {
                flag1 = false;
                await Shell.Current?.DisplayAlert("邮箱错误", "请输入正确的邮箱", "确定");
            }
        }
    }

    internal void MailEditorCaptcha_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (flag2)
        {
            if (VerifyCode == mailCaptcha) { flag3 = true; }
            else { flag3 = false; }
        }
    }

    [RelayCommand]
    private async Task GetVerifyCode()
    {
        if (flag1)
        {
            ViewPage.getVerifyCodeButton.IsEnabled = false;
            int countDown = 60;
            var timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                countDown--;
                if (countDown <= 0)
                {
                    ViewPage.getVerifyCodeButton.IsEnabled = true;
                    timer.Dispose();
                    return;
                }
                VerifyCode = countDown.ToString();
            };
            var randomCode = new Random().Next(100000, 999999).ToString();
            this.mailCaptcha = randomCode;
            var xFEMail = new XFEMail();
            try
            {
                /*xFEMail.SendEmail("验证邮箱", $"您正在修改XCG聊天室的绑定邮箱\n验证码为:{randomCode}\n"
                + "如果不是您本人的操作，请尽快修改密码");*/
                flag2 = true;
            }
            catch (Exception ex)
            {
                await Shell.Current?.DisplayAlert("无法发送邮件", $"错误：{ex.Message}", "退出");
            }
        }
    }

    [RelayCommand]
    private async Task Confirm()
    {
        if (flag1 && flag3)
        {
            UserInfoPage.EditUserProperty(UserPropertyToEdit.Mail, NewMail, ViewPage);
            PrimeMail = UserInfoProfile.CurrentUser.Amail;
            await Shell.Current?.DisplayAlert("修改成功", "您的邮箱已修改成功", "确定");
            Shell.Current.SendBackButtonPressed();
        }
        else
        {
            if (flag1) await Shell.Current?.DisplayAlert("验证码错误", "验证码不匹配", "确定");
            else
            {
                await Shell.Current?.DisplayAlert("邮箱号错误", "邮箱号不正确", "确定");
            }
        }
    }
}
