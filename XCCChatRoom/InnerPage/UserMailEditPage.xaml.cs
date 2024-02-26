using XFE各类拓展.NetCore.MailExtension;
using XFE各类拓展.NetCore.StringExtension;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.InnerPage;

public partial class UserMailEditPage : ContentPage
{
    private bool flag1 = false;
    private bool flag2 = false;
    private bool flag3 = false; /*确认按钮允许点击的条件*/
    private string MailCaptcha = null;
    private string new_Mail = null;

    public UserMailEditPage()
	{
		InitializeComponent();
        OldMail.Text = UserInfo.CurrentUser.Amail;
	}

    private async void NewMail_Unfocused(object sender, FocusEventArgs e)
    {
        if (NewMail.Text is not null)
        {
            if (NewMail.Text.IsValidEmail()) { flag1 = true; }
            else
            {
                flag1 = false;
                await DisplayAlert("邮箱错误", "请输入正确的邮箱", "确定");
            }
        }
    }

    private void MailEditorCaptcha_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(flag2) 
        {
            if (MailEditorCaptcha.Text == this.MailCaptcha) { flag3 = true; }
            else { flag3 = false; }
        }
    }

    private async void GetMailCode_Clicked(object sender, EventArgs e)
    {
        if (flag1)
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
            var randomCode = new Random().Next(100000, 999999).ToString();
            this.MailCaptcha = randomCode;
            var xFEMail = new XFEMail();
            try
            {
                /*xFEMail.SendEmail("验证邮箱", $"您正在修改XCG聊天室的绑定邮箱\n验证码为:{randomCode}\n"
                + "如果不是您本人的操作，请尽快修改密码");*/
                new_Mail = NewMail.Text;
                flag2 = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("无法发送邮件", $"错误：{ex.Message}", "退出");
            }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (flag1 && flag3)
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.Mail, new_Mail, this);
            OldMail.Text = UserInfo.CurrentUser.Amail;
            await DisplayAlert("修改成功", "您的邮箱已修改成功", "确定");
            Shell.Current.SendBackButtonPressed();
        }
        else
        {
            if (flag1) await DisplayAlert("验证码错误", "验证码不匹配", "确定");
            else
            {
                await DisplayAlert("邮箱号错误", "邮箱号不正确", "确定");
            }
        }
    }
}