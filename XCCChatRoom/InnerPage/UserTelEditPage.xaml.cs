using XCCChatRoom.AllImpl;
using XFE各类拓展.NetCore.XFEDataBase;
using XFE各类拓展.StringExtension;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.InnerPage;

public partial class UserTelEditPage : ContentPage
{
    private bool telNumberUniqueBindingCheck = false;
    private bool captchaSendCheck = false;
    private bool captchaVerifyCheck = false;/*确认按钮允许点击的条件*/
    private string telCaptcha = null;
    private string newTelNumber = null;
    public UserTelEditPage()
    {
        InitializeComponent();
        OldTelLabel.Text = UserInfo.CurrentUser.Atel;
    }

    private async void NewTel_Unfocused(object sender, FocusEventArgs e)
    {
        if (NewTelLabel is not null)
        {
            if (NewTelLabel.Text.IsMobPhoneNumber())
            {
                var xFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
                var result = await xFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == NewTelLabel.Text);
                if (result is null)
                {
                    telNumberUniqueBindingCheck = true;
                }
                else 
                { 
                    telNumberUniqueBindingCheck = false;
                    await DisplayAlert("失败", "该手机号已有绑定账号", "确定"); 
                }
            }
            else 
            {
                telNumberUniqueBindingCheck = false;
                await DisplayAlert("出错了捏", "输入手机号不合规哦", "知道啦");
            }
        }
        else {  telNumberUniqueBindingCheck = false;}
    }

    private async void GetTelCodeButton_Clicked(object sender, EventArgs e)
    {
        if (telNumberUniqueBindingCheck)
        {
            GetTelCodeButton.IsEnabled = false;
            int countDown = 60;
            Timer timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                countDown--;
                if (countDown <= 0)
                {
                    GetTelCodeButton.IsEnabled = true;
                    timer.Dispose();
                    return;
                }
                GetTelCodeButton.Text = countDown.ToString();
            };
            var randomCode = new Random().Next(100000, 999999).ToString();
            this.telCaptcha = randomCode;
            await TencentSms.SendVerifyCode("1922760", "+86" + NewTelLabel.Text, [randomCode]);
            newTelNumber = NewTelLabel.Text;
            captchaSendCheck = true;
        }
        else { await DisplayAlert("错误", "请先输入手机号", "确定"); }
    }
    private void TelEditCaptchaEntry_TextChange(object sender, TextChangedEventArgs e)
    {
        if (captchaSendCheck)
        {
            if (TelEditCaptchaEntry.Text == this.telCaptcha) { captchaVerifyCheck = true; }
            else { captchaVerifyCheck = false; }
        }
    }

    private async void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        if (telNumberUniqueBindingCheck && captchaVerifyCheck) 
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.PhoneNum, newTelNumber, this);
            OldTelLabel.Text = UserInfo.CurrentUser.Atel;
            await DisplayAlert("修改成功", "您的手机号已修改成功", "确定");
            Shell.Current.SendBackButtonPressed();
        }
        else {
            if (telNumberUniqueBindingCheck)  await DisplayAlert("验证码错误", "验证码不匹配", "确定"); 
            else await DisplayAlert("手机号错误", "请输入正确的手机号", "确定");
        }
    }
}