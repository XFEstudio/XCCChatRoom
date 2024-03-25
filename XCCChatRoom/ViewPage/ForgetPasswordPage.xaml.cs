using XCCChatRoom.AllImpl;
using XCCChatRoom.Model;
using XFE各类拓展.NetCore.XFEDataBase;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.ViewPage;

public partial class ForgetPasswordPage : ContentPage
{
    private bool telIsValid = false;
    private bool passwordIsValid = false;
    private bool captchaIsValid = false;
    /*确认按钮允许点击的条件*/
    private string telCaptcha = null;
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
                if (result.Count == 1) { telIsValid = true; }
                else
                {
                    await Shell.Current?.DisplayAlert("异常", "该手机号绑定过多个账号", "确定");
                    telIsValid = false;
                }
            }
            else
            {
                await Shell.Current?.DisplayAlert("不存在该用户", "该手机号未绑定过账号", "确定");
                telIsValid = false;
            }
        }
        else 
        {
            telIsValid = false;
            await Shell.Current?.DisplayAlert("错误", "请先输入手机号", "确定"); 
        }
    }

    private async void PasswordQualifiedJudgment()
    {
        passwordIsValid = NewPasswordEditor.Text.PasswordEditor();
        if(!passwordIsValid)
        {
            await Shell.Current?.DisplayAlert("密码不符合规范", "请修改密码", "确定");
        }
    }

    private async void GetTelCode_Clicked(object sender, EventArgs e)
    {
        TelBindJudgment();
        if (telIsValid)
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
            this.telCaptcha = randomCode;
            await TencentSms.SendVerifyCode("1922760", "+86" + Tel.Text, new string[] { randomCode });
            captchaIsValid = false;
        }
    }

    private async void CaptchaJudgment()
    {
        if (ForgetPassword_TelCaptchaEntry is not null)
            if (ForgetPassword_TelCaptchaEntry.Text == this.telCaptcha)
                captchaIsValid = true;
            else { captchaIsValid = false; }
        else 
        { 
            await Shell.Current?.DisplayAlert("警告", "您未输入验证码", "确定");
            captchaIsValid = false; 
        }
    }
    private async void  Button_Clicked(object sender, EventArgs e)
    {
        CaptchaJudgment();
        PasswordQualifiedJudgment();
        if (!passwordIsValid)
            await Shell.Current?.DisplayAlert("警告", "您未输入密码", "确定");
        else if(!captchaIsValid)
            await Shell.Current?.DisplayAlert("警告", "验证码错误", "确定");
        else
        {
            UserInfoPage.EditUserProperty(UserPropertyToEdit.Password, NewPasswordEditor.Text, this);
            await Shell.Current?.DisplayAlert("提示", "修改成功", "返回");
            Shell.Current.SendBackButtonPressed();
        }
        
    }
}