using CommunityToolkit.Mvvm.ComponentModel;
using MauiPopup;
using System.Diagnostics;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.TaskExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewModel;

internal partial class UserRegisterPageViewModel:ObservableObject
{
    [ObservableProperty]
    private string phoneNum;
    [ObservableProperty]
    private string verifyCode;
    [ObservableProperty]
    private string name;
    [ObservableProperty]
    private string mail;
    [ObservableProperty]
    private string password;
    [ObservableProperty]
    private string confirmPassword;
    public UserRegisterPage ViewPage { get; init; }
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    private bool isTelEditor = false, isMailEditor = false, isNameEditor = false, isPasswordEditor = false, isPasswordEnsureEditor = false;
    private string randomCode = string.Empty;
    private string currentPhoneNum = string.Empty;
    private bool isCoolDown = false;

    public UserRegisterPageViewModel(UserRegisterPage viewPage)
    {
        ViewPage = viewPage;
        new Action(() =>
        {
            Thread.Sleep(500);
            while (!ViewPage.userTelEditor.IsFocused)
            {
                ViewPage.userTelEditor.Focus();
                Thread.Sleep(100);
            }
        }).StartNewTask();
    }

    internal async void NextStepButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        try
        {
            ViewPage.telVerifyCodeBorder.Stroke = Color.FromArgb("#444654");
            ViewPage.telVerifyCodeLabel.TextColor = Color.Parse("Gray");
            ViewPage.telVerifyCodeLabel.Text = "验证码";
            ViewPage.userTelBorder.Stroke = Color.FromArgb("#444654");
            ViewPage.userTelLabel.TextColor = Color.Parse("Gray");
            ViewPage.userTelLabel.Text = "手机号";
            if (VerifyCode == randomCode)
            {
                if (PhoneNum == currentPhoneNum)
                {
                    var telResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == PhoneNum);
                    if (telResult.Count > 0)
                    {
                        ViewPage.userTelLabel.Text = "手机号已存在";
                        ViewPage.userTelLabel.TextColor = Color.Parse("Red");
                        ViewPage.userTelBorder.Stroke = Color.Parse("Red");
                        ControlExtension.BorderShake(ViewPage.userTelBorder);
                        ViewPage.userTelEditor.Focus();
                        e.Continue();
                        return;
                    }
                    else
                    {
                        #region FadeAnimation
                        _ = ViewPage.userTelLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        _ = ViewPage.userTelBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        _ = ViewPage.nextStepButton.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        _ = ViewPage.telVerifyCodeButton.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        _ = ViewPage.telVerifyCodeLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        _ = ViewPage.telVerifyCodeBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
                        _ = ViewPage.userTelLabel.FadeTo(0, 800, Easing.SpringOut);
                        _ = ViewPage.userTelBorder.FadeTo(0, 800, Easing.SpringOut);
                        _ = ViewPage.telVerifyCodeLabel.FadeTo(0, 800, Easing.SpringOut);
                        _ = ViewPage.telVerifyCodeBorder.FadeTo(0, 800, Easing.SpringOut);
                        _ = ViewPage.nextStepButton.FadeTo(0, 800, Easing.SpringOut);
                        _ = ViewPage.swtichToLoginPageButton.FadeTo(0, 800, Easing.SpringOut);
                        await ViewPage.telVerifyCodeButton.FadeTo(0, 800, Easing.SpringOut);
                        #endregion
                        #region SetInvisible
                        ViewPage.userTelLabel.IsVisible = false;
                        ViewPage.userTelEditor.IsEnabled = false;
                        ViewPage.userTelBorder.IsVisible = false;
                        ViewPage.telVerifyCodeLabel.IsVisible = false;
                        ViewPage.telVerifyCodeEditor.IsEnabled = false;
                        ViewPage.telVerifyCodeBorder.IsVisible = false;
                        ViewPage.nextStepButton.IsVisible = false;
                        ViewPage.nextStepButton.IsEnabled = false;
                        ViewPage.telVerifyCodeButton.IsVisible = false;
                        #endregion
                        #region SetVisible
                        ViewPage.userNameLabel.IsVisible = true;
                        ViewPage.userNameBorder.IsVisible = true;
                        ViewPage.userPasswordLabel.IsVisible = true;
                        ViewPage.userPasswordBorder.IsVisible = true;
                        ViewPage.userPasswordEnsureLabel.IsVisible = true;
                        ViewPage.userPasswordEnsureBorder.IsVisible = true;
                        ViewPage.userMailLabel.IsVisible = true;
                        ViewPage.userMailBorder.IsVisible = true;
                        ViewPage.userRegisterButton.IsVisible = true;
                        #endregion
                        #region ShowAnimation
                        await new Action(() =>
                        {
                            ViewPage.Dispatcher.Dispatch(() =>
                            {
                                ViewPage.userNameLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                                ViewPage.userNameLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                                ViewPage.userNameBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                                ViewPage.userNameBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            ViewPage.Dispatcher.Dispatch(() =>
                            {
                                ViewPage.userMailLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                                ViewPage.userMailLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                                ViewPage.userMailBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                                ViewPage.userMailBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            ViewPage.Dispatcher.Dispatch(() =>
                            {
                                ViewPage.userPasswordLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                                ViewPage.userPasswordLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                                ViewPage.userPasswordBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                                ViewPage.userPasswordBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            ViewPage.Dispatcher.Dispatch(() =>
                            {
                                ViewPage.userPasswordEnsureLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                                ViewPage.userPasswordEnsureLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                                ViewPage.userPasswordEnsureBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                                ViewPage.userPasswordEnsureBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            ViewPage.Dispatcher.Dispatch(() =>
                            {
                                ViewPage.userRegisterButton.FadeTo(1, 1000, Easing.CubicOut);
                                ViewPage.userRegisterButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                            ViewPage.Dispatcher.Dispatch(() =>
                            {
                                ViewPage.swtichToLoginPageButton.FadeTo(1, 1000, Easing.CubicOut);
                            });
                            Thread.Sleep(200);
                        }).StartNewTask();
                        #endregion
                        ViewPage.userNameEditor.Focus();
                        e.Continue();
                    }
                }
                else
                {
                    ViewPage.userTelLabel.TextColor = Color.Parse("Red");
                    ViewPage.userTelBorder.Stroke = Color.Parse("Red");
                    ViewPage.userTelLabel.Text = "手机号与验证码发送时的不一致";
                    ViewPage.userTelEditor.Focus();
                    e.Continue();
                    ControlExtension.BorderShake(ViewPage.userTelBorder);
                }
            }
            else
            {
                ViewPage.telVerifyCodeLabel.TextColor = Color.Parse("Red");
                ViewPage.telVerifyCodeBorder.Stroke = Color.Parse("Red");
                ViewPage.telVerifyCodeLabel.Text = "验证码错误";
                ViewPage.telVerifyCodeEditor.Focus();
                e.Continue();
                ControlExtension.BorderShake(ViewPage.telVerifyCodeBorder);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current?.DisplayAlert("注册时出现错误", ex.Message, "确定");
        }
    }

    internal async void TelVerifyCodeButton_Clicked(object sender, EventArgs e)
    {
        randomCode = IDGenerator.SummonRandomID(6);
        currentPhoneNum = PhoneNum;
        ViewPage.telVerifyCodeButton.IsEnabled = false;
        ViewPage.telVerifyCodeButton.BackgroundColor = Color.FromArgb("#A491E8");
        ViewPage.telVerifyCodeButton.Text = "发送中...";
        isCoolDown = true;
        var resp = await TencentSms.SendVerifyCode("1922756", "+86" + PhoneNum, [randomCode, "2"]);
        if (resp == null || resp.SendStatusSet.First().Code != "Ok")
        {
            await Shell.Current?.DisplayAlert("出错啦！", $"验证码发送失败：{resp?.SendStatusSet.First().Message}\n手机号：{PhoneNum}", "啊？");
            ViewPage.telVerifyCodeButton.IsEnabled = true;
            ViewPage.telVerifyCodeButton.BackgroundColor = Color.FromArgb("#512BD4");
            ViewPage.telVerifyCodeButton.Text = "重新发送";
        }
        else
        {
            ViewPage.telVerifyCodeButton.Text = "重新发送 60";
            await new Action(() =>
            {
                int timer = 60;
                ViewPage.telVerifyCodeButton.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    ViewPage.telVerifyCodeButton.Text = $"重新发送 {--timer}";
                    if (timer == 0)
                    {
                        ViewPage.telVerifyCodeButton.Text = "重新发送";
                        ViewPage.telVerifyCodeButton.IsEnabled = true;
                        isCoolDown = false;
                        ViewPage.telVerifyCodeButton.BackgroundColor = Color.FromArgb("#512BD4");
                        return false;
                    }
                    return true;
                });
            }).StartNewTask();
        }
    }

    internal async void UserRegisterButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        string UUID = await IDGenerator.GetCorrectUserUID(XCCDataBase.XFEDataBase.CreateExecuter());
        try
        {
            var mailResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Amail == Mail);
            if (mailResult.Count > 0)
            {
                ViewPage.userMailLabel.Text = "邮箱已存在";
                ViewPage.userMailLabel.TextColor = Color.Parse("Red");
                ViewPage.userMailBorder.Stroke = Color.Parse("Red");
                ControlExtension.BorderShake(ViewPage.userMailBorder);
                ViewPage.userMailEditor.Focus();
                return;
            }
            var result = await XFEExecuter.ExecuteAdd(new XFEChatRoom_UserInfoForm()
            {
                ID = UUID,
                Aname = Name,
                Atel = PhoneNum,
                Amail = Mail,
                Apassword = Password,
            });
            if (result == 0)
            {
                await PopupAction.DisplayPopup(new ErrorPopup("注册失败", "未能成功注册，请重试"));
                e.Continue();
                return;
            }
            var successfulLabel = new Label
            {
                Text = "注册成功",
                Opacity = 0,
                TextColor = Color.Parse("White"),
                FontSize = 40,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            ViewPage.Content = successfulLabel;
            await successfulLabel.FadeTo(1, 800, Easing.CubicOut);
            await Task.Delay(500);
            await successfulLabel.FadeTo(0, 800, Easing.CubicOut);
            Shell.Current.SendBackButtonPressed();
            e.Continue();
        }
        catch (Exception ex)
        {
            try
            {
                await XFEExecuter.ExecuteDelete<XFEChatRoom_UserInfoForm>(x => x.ID == UUID);
            }
            catch (Exception) { }
            if (await Shell.Current?.DisplayAlert("注册出错啦！", $"注册失败：{ex.Message}", "重试", "返回"))
            {
                Trace.WriteLine($"手机号：{PhoneNum}");
                Trace.WriteLine($"邮箱：{Mail}");
                Trace.WriteLine($"密码：{Password}");
                Trace.WriteLine($"确认密码：{ConfirmPassword}");
                Trace.WriteLine($"验证码：{VerifyCode}");
                Trace.WriteLine($"随机码：{randomCode}");
                Trace.WriteLine($"用户名：{Name}");
                Trace.WriteLine(ex.ToString());
                e.Continue();
            }
            else
            {
                await Shell.Current.GoToAsync("..");
                e.Continue();
            }
        }
    }

    internal async void SwitchToLoginPageButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
        e.Continue();
    }
    #region 编辑框内容检测
    internal void UserTelEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (PhoneNum.IsMobPhoneNumber())
        {
            isTelEditor = true;
            ViewPage.userTelLabel.Text = "手机号";
            ViewPage.userTelLabel.TextColor = Color.Parse("Black");
            ViewPage.userTelBorder.Stroke = Color.FromArgb("#444654");
            if (!isCoolDown)
            {
                ViewPage.telVerifyCodeButton.IsEnabled = true;
                ViewPage.telVerifyCodeButton.BackgroundColor = Color.FromArgb("#512BD4");
            }
        }
        else
        {
            isTelEditor = false;
            ViewPage.userTelLabel.Text = "手机号格式不正确";
            ViewPage.userTelLabel.TextColor = Color.Parse("Red");
            ViewPage.telVerifyCodeButton.IsEnabled = false;
            ViewPage.telVerifyCodeButton.BackgroundColor = Color.FromArgb("#A491E8");
        }
    }

    internal void UserMailEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Mail.IsValidEmail())
        {
            isMailEditor = true;
            ViewPage.userMailLabel.Text = "邮箱";
            ViewPage.userMailLabel.TextColor = Color.Parse("Black");
            ViewPage.userMailBorder.Stroke = Color.FromArgb("#444654");
        }
        else
        {
            isMailEditor = false;
            ViewPage.userMailLabel.Text = "邮箱格式不正确";
            ViewPage.userMailLabel.TextColor = Color.Parse("Red");
        }
        CheckCorrect();
    }

    internal void UserNameEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            if (!Name.Contains(' '))
            {
                isNameEditor = true;
                ViewPage.userNameLabel.Text = "昵称";
                ViewPage.userNameLabel.TextColor = Color.Parse("Black");
            }
            else
            {
                ViewPage.userNameLabel.Text = "昵称不可包含空格";
                ViewPage.userNameLabel.TextColor = Color.Parse("Red");
                isNameEditor = false;
            }
        }
        else
        {
            ViewPage.userNameLabel.Text = "昵称不可为空";
            ViewPage.userNameLabel.TextColor = Color.Parse("Red");
            isNameEditor = false;
        }
        CheckCorrect();
    }

    internal void UserPasswordEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Password))
        {
            if (!Password.Contains(' '))
            {
                isPasswordEditor = true;
                ViewPage.userPasswordLabel.Text = "密码";
                ViewPage.userPasswordLabel.TextColor = Color.Parse("Black");
            }
            else
            {
                ViewPage.userPasswordLabel.Text = "密码不可包含空格";
                ViewPage.userPasswordLabel.TextColor = Color.Parse("Red");
                isPasswordEditor = false;
            }
        }
        else
        {
            ViewPage.userPasswordLabel.Text = "密码不可为空";
            ViewPage.userPasswordLabel.TextColor = Color.Parse("Red");
            isPasswordEditor = false;
        }
        CheckCorrect();
    }

    internal void UserPasswordEnsureEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ConfirmPassword == Password)
        {
            isPasswordEnsureEditor = true;
            ViewPage.userPasswordEnsureLabel.Text = "确认密码";
            ViewPage.userPasswordEnsureLabel.TextColor = Color.Parse("Black");
        }
        else
        {
            isPasswordEnsureEditor = false;
            ViewPage.userPasswordEnsureLabel.Text = "两次密码不一致";
            ViewPage.userPasswordEnsureLabel.TextColor = Color.Parse("Red");
        }
        CheckCorrect();
    }

    internal void TelVerifyCodeEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (isTelEditor && VerifyCode.Length == 6)
        {
            if (!ViewPage.nextStepButton.IsWaiting)
            {
                ViewPage.nextStepButton.BackgroundColor = Color.Parse("#512BD4");
                ViewPage.nextStepButton.IsEnabled = true;
            }
            TelVerifyCodeEditor_Unfocused(null, null);
        }
        else
        {
            if (!ViewPage.nextStepButton.IsWaiting)
            {
                ViewPage.nextStepButton.BackgroundColor = Color.Parse("#A491E8");
                ViewPage.nextStepButton.IsEnabled = false;
            }
        }
    }

    internal void CheckCorrect()
    {
        if (isTelEditor && isMailEditor && isNameEditor && isPasswordEditor && isPasswordEnsureEditor)
        {
            ViewPage.userRegisterButton.BackgroundColor = Color.Parse("#512BD4");
            if (!ViewPage.userRegisterButton.IsWaiting)
                ViewPage.userRegisterButton.IsEnabled = true;
        }
        else
        {
            ViewPage.userRegisterButton.BackgroundColor = Color.Parse("#A491E8");
            if (!ViewPage.userRegisterButton.IsWaiting)
                ViewPage.userRegisterButton.IsEnabled = false;
        }
    }
    #endregion
    #region 编辑框焦点事件
    internal void UserTelEditor_Focused(object sender, FocusEventArgs e)
    {
        ViewPage.userTelLabel.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userTelLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        ViewPage.userTelBorder.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userTelBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    internal void UserTelEditor_Unfocused(object sender, FocusEventArgs e)
    {
        ViewPage.userTelLabel.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userTelLabel.ScaleTo(1, 300, Easing.CubicOut);
        ViewPage.userTelBorder.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userTelBorder.ScaleTo(1, 300, Easing.CubicOut);
    }

    internal void UserMailEditor_Focused(object sender, FocusEventArgs e)
    {
        ViewPage.userMailLabel.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userMailLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        ViewPage.userMailBorder.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userMailBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    internal void UserMailEditor_Unfocused(object sender, FocusEventArgs e)
    {
        ViewPage.userMailLabel.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userMailLabel.ScaleTo(1, 300, Easing.CubicOut);
        ViewPage.userMailBorder.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userMailBorder.ScaleTo(1, 300, Easing.CubicOut);
    }

    internal void UserNameEditor_Focused(object sender, FocusEventArgs e)
    {
        ViewPage.userNameLabel.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userNameLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        ViewPage.userNameBorder.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userNameBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    internal void UserNameEditor_Unfocused(object sender, FocusEventArgs e)
    {
        ViewPage.userNameLabel.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userNameLabel.ScaleTo(1, 300, Easing.CubicOut);
        ViewPage.userNameBorder.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userNameBorder.ScaleTo(1, 300, Easing.CubicOut);
    }

    internal void TelVerifyCodeEditor_Focused(object sender, FocusEventArgs e)
    {
        ViewPage.telVerifyCodeLabel.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.telVerifyCodeLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        ViewPage.telVerifyCodeBorder.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.telVerifyCodeBorder.ScaleTo(1.2, 300, Easing.CubicOut);
        ViewPage.telVerifyCodeButton.ScaleTo(0.8, 300, Easing.CubicOut);
    }

    internal void TelVerifyCodeEditor_Unfocused(object sender, FocusEventArgs e)
    {
        ViewPage.telVerifyCodeLabel.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.telVerifyCodeLabel.ScaleTo(1, 300, Easing.CubicOut);
        ViewPage.telVerifyCodeBorder.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.telVerifyCodeBorder.ScaleTo(1, 300, Easing.CubicOut);
        ViewPage.telVerifyCodeButton.ScaleTo(1, 300, Easing.CubicOut);
    }

    internal void UserPasswordEditor_Focused(object sender, FocusEventArgs e)
    {
        ViewPage.userPasswordLabel.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userPasswordLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        ViewPage.userPasswordBorder.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userPasswordBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    internal void UserPasswordEditor_Unfocused(object sender, FocusEventArgs e)
    {
        ViewPage.userPasswordLabel.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userPasswordLabel.ScaleTo(1, 300, Easing.CubicOut);
        ViewPage.userPasswordBorder.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userPasswordBorder.ScaleTo(1, 300, Easing.CubicOut);
    }

    internal void UserPasswordEnsureEditor_Focused(object sender, FocusEventArgs e)
    {
        ViewPage.userPasswordEnsureLabel.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userPasswordEnsureLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        ViewPage.userPasswordEnsureBorder.FadeTo(1, 300, Easing.CubicOut);
        ViewPage.userPasswordEnsureBorder.ScaleTo(1.2, 300, Easing.CubicOut);
    }

    internal void UserPasswordEnsureEditor_Unfocused(object sender, FocusEventArgs e)
    {
        ViewPage.userPasswordEnsureLabel.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userPasswordEnsureLabel.ScaleTo(1, 300, Easing.CubicOut);
        ViewPage.userPasswordEnsureBorder.FadeTo(0.5, 300, Easing.CubicOut);
        ViewPage.userPasswordEnsureBorder.ScaleTo(1, 300, Easing.CubicOut);
    }
    #endregion
}
