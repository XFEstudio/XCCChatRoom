using MauiPopup;
using MauiPopup.Views;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.TaskExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.Controls;

public partial class UserTelEditPopup : BasePopupPage
{
    private bool isTelChanged = false, isTelEditorEmpty = true;
    private bool isCoolDown = false;
    private string currentNewPhoneNum = string.Empty;
    private string randomCode = string.Empty;
    private readonly UserPropertyEditPage userPropertyEditPage;
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    public UserTelEditPopup(UserPropertyEditPage userPropertyEditPage)
    {
        this.userPropertyEditPage = userPropertyEditPage;
        InitializeComponent();
        new Action(() =>
        {
            Thread.Sleep(500);
            while (!UserTelEditor.IsFocused)
            {
                UserTelEditor.Dispatcher.Dispatch(() => UserTelEditor.Focus());
                Thread.Sleep(100);
            }
        }).StartNewTask();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (UserTelEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (TelVerifyCodeEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    private void TelVerifyCodeEditor_Focused(object sender, FocusEventArgs e)
    {
        TelVerifyCodeLabel.FadeTo(1, 300, Easing.CubicOut);
        TelVerifyCodeLabel.ScaleTo(1.1, 300, Easing.CubicOut);
        TelVerifyCodeBorder.FadeTo(1, 300, Easing.CubicOut);
        TelVerifyCodeBorder.ScaleTo(1.1, 300, Easing.CubicOut);
        TelVerifyCodeButton.ScaleTo(0.8, 300, Easing.CubicOut);
    }

    private void TelVerifyCodeEditor_Unfocused(object sender, FocusEventArgs e)
    {
        TelVerifyCodeLabel.FadeTo(0.5, 300, Easing.CubicOut);
        TelVerifyCodeLabel.ScaleTo(1, 300, Easing.CubicOut);
        TelVerifyCodeBorder.FadeTo(0.5, 300, Easing.CubicOut);
        TelVerifyCodeBorder.ScaleTo(1, 300, Easing.CubicOut);
        TelVerifyCodeButton.ScaleTo(1, 300, Easing.CubicOut);
    }
    private void UserTelEditor_Focused(object sender, FocusEventArgs e)
    {
        if (!isTelChanged)
        {
            var animation = new Animation(v => UserTelBorder.MaximumWidthRequest = v, 100, 300);
            var animation2 = new Animation(v => UserTelLabel.MaximumWidthRequest = v, 100, 300);
            animation.Commit(this, "UserAccountBorderWidthAnimation", 16, 300, Easing.CubicOut);
            animation2.Commit(this, "UserAccountLabelWidthAnimation", 16, 300, Easing.CubicOut);
            UserTelLabel.FadeTo(1, 300, Easing.CubicOut);
            UserTelBorder.FadeTo(1, 300, Easing.CubicOut);
            isTelChanged = true;
        }
    }

    private void UserTelEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (UserTelEditor.Text is null || UserTelEditor.Text == string.Empty)
        {
            if (isTelChanged)
            {
                var animation = new Animation(v => UserTelBorder.MaximumWidthRequest = v, 300, 100);
                var animation2 = new Animation(v => UserTelLabel.MaximumWidthRequest = v, 300, 100);
                animation.Commit(this, "UserAccountBorderWidthAnimation", 16, 200, Easing.CubicOut);
                animation2.Commit(this, "UserAccountLabelWidthAnimation", 16, 200, Easing.CubicOut);
                UserTelLabel.FadeTo(0.5, 300, Easing.CubicOut);
                UserTelBorder.FadeTo(0.5, 200, Easing.CubicOut);
                isTelChanged = false;
            }
        }
    }

    private void TelVerifyCodeEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!isTelEditorEmpty && TelVerifyCodeEditor.Text.Length == 6)
        {
            if (!SaveAndBackButton.IsWaiting)
            {
                SaveAndBackButton.SetDynamicResource(Button.BackgroundColorProperty, "MainColor");
                SaveAndBackButton.IsEnabled = true;
            }
            TelVerifyCodeEditor_Unfocused(null, null);
        }
        else
        {
            if (!SaveAndBackButton.IsWaiting)
            {
                SaveAndBackButton.BackgroundColor = Color.Parse("#A491E8");
                SaveAndBackButton.IsEnabled = false;
            }
        }
    }

    private void UserTelEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (UserTelEditor.Text.IsMobPhoneNumber())
        {
            if (UserTelEditor.Text != userPropertyEditPage.ViewModel.PhoneNum)
            {
                isTelEditorEmpty = false;
                UserTelLabel.Text = "手机号";
                UserTelLabel.SetDynamicResource(Label.TextColorProperty, "NormalTextColor");
                UserTelBorder.SetDynamicResource(Border.StrokeProperty, "DeepDarkGrayColor");
                if (!isCoolDown)
                {
                    TelVerifyCodeButton.IsEnabled = true;
                    TelVerifyCodeButton.SetDynamicResource(Button.BackgroundColorProperty, "MainColor");
                }
            }
            else
            {
                isTelEditorEmpty = true;
                UserTelLabel.Text = "新手机号不能与原手机号一致";
                UserTelLabel.TextColor = Color.Parse("Red");
                TelVerifyCodeButton.IsEnabled = false;
                TelVerifyCodeButton.SetDynamicResource(Button.BackgroundColorProperty, "DisabledMainColor");
            }
        }
        else
        {
            isTelEditorEmpty = true;
            UserTelLabel.Text = "手机号格式不正确";
            UserTelLabel.TextColor = Color.Parse("Red");
            TelVerifyCodeButton.IsEnabled = false;
            TelVerifyCodeButton.SetDynamicResource(Button.BackgroundColorProperty, "DisabledMainColor");
        }
    }

    private async void SaveAndBackButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        if (currentNewPhoneNum == UserTelEditor.Text)
        {
            if (TelVerifyCodeEditor.Text == randomCode)
            {
                if (await XFEExecuter.ExecuteGetFirst<XFEChatRoom_UserInfoForm>(x => x.Atel == currentNewPhoneNum) is null)
                {
                    UserInfoProfile.CurrentUser.Atel = currentNewPhoneNum;
                    if (await UserInfoPage.UpLoadUserInfo() > 0)
                    {
                        userPropertyEditPage.ViewModel.PhoneNum = currentNewPhoneNum;
                        SendBackButtonPressed();
                        await PopupAction.DisplayPopup(new TipPopup("手机号修改成功"));
                    }
                    else
                    {
                        await PopupAction.DisplayPopup(new ErrorPopup("手机号修改失败"));
                    }
                    e.Continue();
                }
                else
                {
                    ControlExtension.BorderShake(TelVerifyCodeBorder);
                    TelVerifyCodeLabel.Text = "手机号已被注册过";
                    TelVerifyCodeLabel.TextColor = Color.Parse("Red");
                    TelVerifyCodeBorder.Stroke = Color.Parse("Red");
                    TelVerifyCodeEditor.Focus();
                    e.Continue();
                }
            }
            else
            {
                ControlExtension.BorderShake(TelVerifyCodeBorder);
                TelVerifyCodeLabel.Text = "验证码不正确";
                TelVerifyCodeLabel.TextColor = Color.Parse("Red");
                TelVerifyCodeBorder.Stroke = Color.Parse("Red");
                TelVerifyCodeEditor.Focus();
                e.Continue();
            }
        }
        else
        {
            ControlExtension.BorderShake(UserTelBorder);
            UserTelLabel.Text = "手机号与发送验证码时不匹配";
            UserTelLabel.TextColor = Color.Parse("Red");
            UserTelBorder.Stroke = Color.Parse("Red");
            UserTelEditor.Focus();
            e.Continue();
        }
    }

    private async void TelVerifyCodeButton_Clicked(object sender, EventArgs e)
    {
        currentNewPhoneNum = UserTelEditor.Text;
        randomCode = IDGenerator.SummonRandomID(6);
        TelVerifyCodeButton.IsEnabled = false;
        TelVerifyCodeButton.SetDynamicResource(Button.BackgroundColorProperty, "DisabledMainColor");
        TelVerifyCodeButton.Text = "发送中...";
        isCoolDown = true;
        var resp = await TencentSms.SendVerifyCode("1922756", "+86" + UserTelEditor.Text, [randomCode, "2"]);
        if (resp is null || resp.SendStatusSet.First().Code != "Ok")
        {
            await Shell.Current!.DisplayAlert("出错啦！", $"验证码发送失败：{resp?.SendStatusSet.First().Message}\n手机号：{UserTelEditor.Text}", "啊？");
            TelVerifyCodeButton.IsEnabled = true;
            TelVerifyCodeButton.SetDynamicResource(Button.BackgroundColorProperty, "MainColor");
            TelVerifyCodeButton.Text = "重新发送";
        }
        else
        {
            TelVerifyCodeButton.Text = "重新发送 60";
            await new Action(() =>
            {
                int timer = 60;
                TelVerifyCodeButton.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    TelVerifyCodeButton.Text = $"重新发送 {--timer}";
                    if (timer == 0)
                    {
                        TelVerifyCodeButton.Text = "重新发送";
                        TelVerifyCodeButton.IsEnabled = true;
                        isCoolDown = false;
                        TelVerifyCodeButton.SetDynamicResource(Button.BackgroundColorProperty, "MainColor");
                        return false;
                    }
                    return true;
                });
            }).StartNewTask();
        }
    }
}