using XFE各类拓展.MailExtension;
using XFE各类拓展.StringExtension;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.InnerPage;

public partial class UserMailEditorPage : ContentPage
{
    private bool flag1 = false;
    private bool flag2 = false; /*确认按钮允许点击的条件*/
    private string MailCaptcha = null;

    public UserMailEditorPage()
	{
		InitializeComponent();
        OldMail.Text = UserInfo.CurrentUser.Amail;
	}

    private void NewMail_Unfocused(object sender, FocusEventArgs e)
    {
        if (NewMail.Text is not null)
        {
            if (NewMail.Text.IsValidEmail()) { flag1 = true; }
        }
    }

    private void MailEditorCaptcha_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (MailEditorCaptcha.Text == this.MailCaptcha) { flag2 = true; }
    }

    private void GetMailCode_Clicked(object sender, EventArgs e)
    {
        GetMailCode.IsEnabled = false;
        int countDown = 60;
        Timer timer = new Timer(1000);
        timer.Elapsed += (sender, e) =>
        {
            countDown--;
            if (countDown <= 0)
            {
                GetMailCode.IsEnabled = true;
                timer.Dispose();
                return;
            }
            GetMailCode.Text = countDown.ToString();
        };
        var randomCode = new Random().Next(0, 999999).ToString();
        this.MailCaptcha = randomCode;
        XFEMail xFEMail = new XFEMail();
        xFEMail.SendEmail("验证邮箱", "您正在修改XCG聊天室的绑定邮箱\n验证码为:");

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (flag1 && flag2)
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.Mail, NewMail.Text, this);
            OldMail.Text = UserInfo.CurrentUser.Amail;
            await DisplayAlert("修改成功", "您的邮箱已修改成功", "确定");
        }
        else
        {
            if (flag1) await DisplayAlert("验证码错误", "验证码不匹配", "确定");
            else await DisplayAlert("邮箱错误", "请输入正确的邮箱", "确定");
        }
    }
}