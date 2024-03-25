using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Model;
using XCCChatRoom.ViewPage;
using XFE各类拓展.NetCore.XFEDataBase;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.ViewModel;

public partial class ForgetPasswordPageViewModel(ForgetPasswordPage viewPage) : ObservableObject
{
    private bool telIsValid = false;
    private bool passwordIsValid = false;
    private bool captchaIsValid = false;
    /*确认按钮允许点击的条件*/
    private string telCaptcha = null;

    [ObservableProperty]
    private string telNumber;
    [ObservableProperty]
    private string password;
    [ObservableProperty]
    private string verifyCode;

    public ForgetPasswordPage ViewPage { get; init; } = viewPage;

    private async void TelBindJudgment()
    {
        if (TelNumber is not null)
        {
            var xFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
            var result = await xFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == TelNumber);
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
        passwordIsValid = Password.IsValidPassword();
        if (!passwordIsValid)
        {
            await Shell.Current?.DisplayAlert("密码不符合规范", "请修改密码", "确定");
        }
    }

    [RelayCommand]
    async Task GetVerifyCode()
    {
        TelBindJudgment();
        if (telIsValid)
        {
            ViewPage.getTelCode.IsEnabled = false;
            int countDown = 60;
            var timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                countDown--;
                if (countDown <= 0)
                {
                    ViewPage.getTelCode.IsEnabled = true;
                    timer.Dispose();
                    return;
                }
                ViewPage.getTelCode.Text = countDown.ToString();
            };
            var randomCode = new Random().Next(1, 999999).ToString();
            if (randomCode.Length < 6)
            {
                for (int i = 6 - randomCode.Length; i > 0; i--)
                    randomCode = $"0{randomCode}";
            }
            telCaptcha = randomCode;
            await TencentSms.SendVerifyCode("1922760", "+86" + TelNumber, new string[] { randomCode });
            captchaIsValid = false;
        }
    }

    private async void CaptchaJudgment()
    {
        if (VerifyCode is not null)
            if (VerifyCode == telCaptcha)
                captchaIsValid = true;
            else { captchaIsValid = false; }
        else
        {
            await Shell.Current?.DisplayAlert("警告", "您未输入验证码", "确定");
            captchaIsValid = false;
        }
    }

    [RelayCommand]
    async Task Confirm()
    {
        CaptchaJudgment();
        PasswordQualifiedJudgment();
        if (!passwordIsValid)
            await Shell.Current?.DisplayAlert("警告", "您未输入密码", "确定");
        else if (!captchaIsValid)
            await Shell.Current?.DisplayAlert("警告", "验证码错误", "确定");
        else
        {
            UserInfoPage.EditUserProperty(UserPropertyToEdit.Password, Password, ViewPage);
            await Shell.Current?.DisplayAlert("提示", "修改成功", "返回");
            Shell.Current.SendBackButtonPressed();
        }

    }
}