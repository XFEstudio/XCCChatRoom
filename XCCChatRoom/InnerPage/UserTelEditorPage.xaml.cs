using XCCChatRoom.AllImpl;
using XFE各类拓展.NetCore.XFEDataBase;
using XFE各类拓展.StringExtension;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.InnerPage;

public partial class UserTelEditorPage : ContentPage
{
    private bool flag1 = false;
    private bool flag2 = false;
    private bool flag3 = false;/*确认按钮允许点击的条件*/
    private string TelCaptcha = null;
    private string new_Tel = null;
    public UserTelEditorPage()
    {
        InitializeComponent();
        OldTel.Text = UserInfo.CurrentUser.Atel;
    }

    private async void NewTel_Unfocused(object sender, FocusEventArgs e)
    {
        if (NewTel is not null)
        {
            if (NewTel.Text.IsMobPhoneNumber())
            {
                var xFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
                var result = await xFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == NewTel.Text);
                if (result is null)
                {
                    flag1 = true;
                }
                else 
                { 
                    flag1 = false;
                    await DisplayAlert("失败", "该手机号已有绑定账号", "确定"); 
                }
            }
            else 
            {
                flag1 = false;
                await DisplayAlert("出错了捏", "输入手机号不合规哦", "知道啦");
            }
        }
        else {  flag1 = false;}
    }

    private async void GetTelCode_Clicked(object sender, EventArgs e)
    {
        if (flag1)
        {
            GetTelCode.IsEnabled = false;
            int countDown = 60;
            Timer timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                countDown--;
                if (countDown <= 0)
                {
                    GetTelCode.IsEnabled = true;
                    timer.Dispose();
                    return;
                }
                GetTelCode.Text = countDown.ToString();
            };
            var randomCode = new Random().Next(0, 999999).ToString();
            if (randomCode.Length < 6)
            {
                for (int i = 6 - randomCode.Length; i > 0; i--)
                    randomCode = $"0{randomCode}";
            }
            this.TelCaptcha = randomCode;
            await TencentSms.SendVerifyCode(this, "1922760", "+86" + NewTel.Text, new string[] { randomCode });
            new_Tel = NewTel.Text;
            flag2 = true;
        }
        else { await DisplayAlert("错误", "请先输入手机号", "确定"); }
    }
    private void TelEditorCaptcha_TextChange(object sender, TextChangedEventArgs e)
    {
        if (flag2)
        {
            if (TelEditorCaptcha.Text == this.TelCaptcha) { flag3 = true; }
            else { flag3 = false; }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (flag1 && flag3) 
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.PhoneNum, new_Tel, this);
            OldTel.Text = UserInfo.CurrentUser.Atel;
            await DisplayAlert("修改成功", "您的手机号已修改成功", "确定");
            Shell.Current.SendBackButtonPressed();
        }
        else {
            if (flag1)  await DisplayAlert("验证码错误", "验证码不匹配", "确定"); 
            else await DisplayAlert("手机号错误", "请输入正确的手机号", "确定");
        }
    }
}