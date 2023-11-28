using XCCChatRoom.AllImpl;
using XFE各类拓展.NetCore.XFEDataBase;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.InnerPage;

public partial class ForgetPasswordPage : ContentPage
{
    private bool telflag = false;
    private bool passwordflag = false;
    private bool captchaflag = false;
    /*确认按钮允许点击的条件*/
    private string TelCaptcha = null;
    public ForgetPasswordPage()
	{
		InitializeComponent();
	}
    
    private async void TelBindJudgment()
    {
        if(Tel.Text is not null)
        {
            var xFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
            var result = await xFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == Tel.Text);
            if (result.Count > 0 && result is not null)
            {
                if (result.Count == 1) { telflag = true; }
                else
                {
                    await DisplayAlert("异常", "该手机号绑定过多个账号", "确定");
                    telflag = false;
                }
            }
            else
            {
                await DisplayAlert("不存在该用户", "该手机号未绑定过账号", "确定");
                telflag = false;
            }
        }
        else 
        {
            telflag = false;
            await DisplayAlert("错误", "请先输入手机号", "确定"); 
        }
    }

    private async void PasswordQualifiedJudgment()
    {
        passwordflag = NewPassword.Text.PasswordEditor();
        if(!passwordflag)
        {
            await DisplayAlert("密码不合格", "请修改密码", "确定");
        }
    }

    private async void GetTelCode_Clicked(object sender, EventArgs e)
    {
        TelBindJudgment();
        if (telflag)
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
            var randomCode = new Random().Next(1, 999999).ToString();
            if (randomCode.Length < 6)
            {
                for (int i = 6 - randomCode.Length; i > 0; i--)
                    randomCode = $"0{randomCode}";
            }
            this.TelCaptcha = randomCode;
            await TencentSms.SendVerifyCode("1922760", "+86" + Tel.Text, new string[] { randomCode });
            captchaflag = false;
        }
    }

    private async void CaptchaJudgment()
    {
        if (ForgetPassword_TelCaptcha is not null)
            if (ForgetPassword_TelCaptcha.Text == this.TelCaptcha)
                captchaflag = true;
            else { captchaflag = false; }
        else 
        { 
            await DisplayAlert("警告", "您未输入验证码", "确定");
            captchaflag = false; 
        }
    }
    private async void  Button_Clicked(object sender, EventArgs e)
    {
        CaptchaJudgment();
        PasswordQualifiedJudgment();
        if (!passwordflag)
            await DisplayAlert("警告", "您未输入密码", "确定");
        else if(!captchaflag)
            await DisplayAlert("警告", "验证码错误", "确定");
        else
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.Password, NewPassword.Text, this);
            await DisplayAlert("提示", "修改成功", "返回");
            Shell.Current.SendBackButtonPressed();
        }
        
    }
}