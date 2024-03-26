using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewModel;

internal partial class UserLoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string userAccountText;
    [ObservableProperty]
    private string userPasswordText;
    [ObservableProperty]
    private string userTelNum;
    [ObservableProperty]
    private string telVerifyCode;
    public UserLoginPage ViewPage { get; init; }
    private readonly XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    private bool isAccountChanged = false, isPasswordChanged = false, isTelChanged = false;
    private bool isPasswordEditorEmpty = true, isAccountEditorEmpty = true, isTelEditorEmpty = true;
    private bool isCoolDown = false;
    private string currentPhoneNum = string.Empty;
    private string randomCode = string.Empty;

    public UserLoginPageViewModel(UserLoginPage viewPage)
    {
        ViewPage = viewPage;

        try
        {
            new Action(async () =>
            {
                if (AppSystemProfile.LoginMethod == LoginMethod.VerifyCodeLogin)
                {
                    await ViewPage.Dispatcher.DispatchAsync(async () => await SwitchToTelVerifyCodeLoginStyle());
                    Thread.Sleep(500);
                    while (!ViewPage.userTelEditor.IsFocused)
                    {
                        ViewPage.userTelEditor.Dispatcher.Dispatch(() => ViewPage.userTelEditor.Focus());
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    Thread.Sleep(500);
                    while (!ViewPage.UserAccountEditor.IsFocused)
                    {
                        ViewPage.userAccountBorder.Dispatcher.Dispatch(() => UserAccountEditor.Focus());
                        Thread.Sleep(100);
                    }
                }
            }).StartNewTask();
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
        }
    }

    private async Task SwitchToTelVerifyCodeLoginStyle()
    {
        AppSystemProfile.LoginMethod = LoginMethod.VerifyCodeLogin;
        AppSystemProfile.SaveSystemProfile();
        #region FadeAnimation
        _ = ViewPage.userAccountLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
        _ = ViewPage.userAccountBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
        _ = ViewPage.ViewPage.userPasswordLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
        _ = ViewPage.ViewPage.userPasswordBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
        _ = ViewPage.userAccountLabel.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.userAccountBorder.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.ViewPage.userPasswordLabel.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.ViewPage.userPasswordBorder.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.switchToTelVerifyCodeLoginButton.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.ViewPage.userLoginButton.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.forgotPasswordButton.FadeTo(0, 800, Easing.SpringOut);
        await ViewPage.ViewPage.switchToRegisterPageButton.FadeTo(0, 800, Easing.SpringOut);
        #endregion
        #region SetInvisible
        ViewPage.userAccountLabel.IsVisible = false;
        ViewPage.userAccountBorder.IsVisible = false;
        ViewPage.ViewPage.userPasswordLabel.IsVisible = false;
        ViewPage.ViewPage.userPasswordBorder.IsVisible = false;
        ViewPage.switchToTelVerifyCodeLoginButton.IsVisible = false;
        #endregion
        #region SetVisible
        ViewPage.userTelLabel.TranslationX = 0;
        ViewPage.userTelLabel.TranslationY = 150;
        ViewPage.userTelLabel.IsVisible = true;
        ViewPage.userTelBorder.TranslationX = 0;
        ViewPage.userTelBorder.TranslationY = 150;
        ViewPage.userTelBorder.IsVisible = true;
        ViewPage.telVerifyCodeLabel.TranslationX = 0;
        ViewPage.telVerifyCodeLabel.TranslationY = 110;
        ViewPage.telVerifyCodeLabel.IsVisible = true;
        ViewPage.telVerifyCodeGrid.TranslationX = 0;
        ViewPage.telVerifyCodeGrid.TranslationY = 110;
        ViewPage.telVerifyCodeGrid.IsVisible = true;
        ViewPage.forgotPasswordButton.TranslationX = 0;
        ViewPage.forgotPasswordButton.TranslationY = 70;
        ViewPage.ViewPage.userLoginButton.TranslationX = 0;
        ViewPage.ViewPage.userLoginButton.TranslationY = 50;
        ViewPage.ViewPage.switchToPasswordLoginButton.TranslationX = 0;
        ViewPage.ViewPage.switchToPasswordLoginButton.TranslationY = 30;
        ViewPage.ViewPage.switchToPasswordLoginButton.IsVisible = true;
        ViewPage.ViewPage.switchToRegisterPageButton.TranslationX = 0;
        ViewPage.ViewPage.switchToRegisterPageButton.TranslationY = 20;
        #endregion
        #region ShowAnimation
        await new Action(() =>
        {
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.userTelLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                ViewPage.userTelBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                ViewPage.userTelLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.userTelBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.telVerifyCodeGrid.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.telVerifyCodeGrid.FadeTo(1, 1000, Easing.CubicOut);
                ViewPage.telVerifyCodeLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                ViewPage.telVerifyCodeLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.telVerifyCodeBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                ViewPage.telVerifyCodeButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                forgotPasswordButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                forgotPasswordButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.userLoginButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.userLoginButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.switchToPasswordLoginButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.switchToPasswordLoginButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.switchToRegisterPageButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.switchToRegisterPageButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
        }).StartNewTask();
        ViewPage.userTelEditor.Focus();
        #endregion
    }

    private async Task ProcessLoginInfo(XFEChatRoom_UserInfoForm userInfo, Controls.WaitButtonClickedEventArgs e)
    {
        switch (AppSystemProfile.LoginMethod)
        {
            case LoginMethod.PasswordLogin:
                if (userInfo.Apassword == UserPasswordText)
                {
                    await SwitchToLoginSuccessfulStyle(userInfo);
                    e.Continue();
                }
                else
                {
                    ControlExtension.BorderShake(ViewPage.userPasswordBorder);
                    ViewPage.userPasswordLabel.Text = "密码输入错误";
                    ViewPage.userPasswordLabel.TextColor = Color.Parse("Red");
                    ViewPage.userPasswordBorder.Stroke = Color.Parse("Red");
                    ViewPage.userPasswordEditor.Text = string.Empty;
                    ViewPage.userPasswordEditor.Focus();
                    e.Continue();
                }
                break;
            case LoginMethod.VerifyCodeLogin:
                await SwitchToLoginSuccessfulStyle(userInfo);
                e.Continue();
                break;
            default:
                ProcessException.ShowEnumException();
                break;
        }
    }

    private async Task SwitchToLoginSuccessfulStyle(XFEChatRoom_UserInfoForm userInfo)
    {
        var successfulLabel = new Label
        {
            Text = "登录成功",
            Opacity = 0,
            TextColor = Color.FromArgb("#ffffff"),
            FontAttributes = FontAttributes.Bold,
            FontSize = 40,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };
        ViewPage.Content = successfulLabel;
        await successfulLabel.FadeTo(1, 800, Easing.CubicOut);
        UserInfoProfile.LoginSuccessful = true;
        UserInfoProfile.Name = userInfo.Aname;
        UserInfoProfile.UUID = userInfo.ID;
        UserInfoProfile.Email = userInfo.Amail;
        UserInfoProfile.Phone = userInfo.Atel;
        UserInfoProfile.Password = userInfo.Apassword;
        if (UserInfoPage.Current is null)
        {
            UserInfoProfile.LoginSuccessful = true;
        }
        else
        {
            UserInfoPage.Current.ViewModel.UserName = userInfo.Aname;
            UserInfoPage.Current.ViewModel.UUID = userInfo.ID;
            UserInfoProfile.CurrentUser = userInfo;
            UserInfoPage.Current.SwitchToLoginStyle();
            CommunityPage.Current?.ViewModel.PostRefresh();
        }
        GroupContactPage.Current.groupRefreshView.IsRefreshing = true;
        await Task.Delay(600);
        await successfulLabel.FadeTo(0, 500, Easing.CubicOut);
        UserInfoPage.Current.Opacity = 0;
        Shell.Current.SendBackButtonPressed();
        await UserInfoPage.Current.FadeTo(1, 300, Easing.CubicOut);
    }
    #region 焦点事件
    internal void UserAccountEditor_Focused(object sender, FocusEventArgs e)
    {
        if (!isAccountChanged)
        {
            var animation = new Animation(v => ViewPage.userAccountBorder.MaximumWidthRequest = v, 100, 300);
            var animation2 = new Animation(v => ViewPage.userAccountLabel.MaximumWidthRequest = v, 100, 300);
            animation.Commit(ViewPage, "UserAccountBorderWidthAnimation", 16, 300, Easing.CubicOut);
            animation2.Commit(ViewPage, "UserAccountLabelWidthAnimation", 16, 300, Easing.CubicOut);
            ViewPage.userAccountLabel.FadeTo(1, 300, Easing.CubicOut);
            ViewPage.userAccountBorder.FadeTo(1, 300, Easing.CubicOut);
            isAccountChanged = true;
        }
    }

    internal void UserAccountEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (UserAccountText is null || UserAccountText == string.Empty)
        {
            if (isAccountChanged)
            {
                var animation = new Animation(v => UserAccountBorder.MaximumWidthRequest = v, 300, 100);
                var animation2 = new Animation(v => UserAccountLabel.MaximumWidthRequest = v, 300, 100);
                animation.Commit(this, "UserAccountBorderWidthAnimation", 16, 200, Easing.CubicOut);
                animation2.Commit(this, "UserAccountLabelWidthAnimation", 16, 200, Easing.CubicOut);
                UserAccountLabel.FadeTo(0.5, 300, Easing.CubicOut);
                UserAccountBorder.FadeTo(0.5, 200, Easing.CubicOut);
                isAccountChanged = false;
            }
        }
    }

    internal void UserPasswordEditor_Focused(object sender, FocusEventArgs e)
    {
        if (!isPasswordChanged)
        {
            var animation = new Animation(v => ViewPage.userPasswordBorder.MaximumWidthRequest = v, 100, 300);
            var animation2 = new Animation(v => ViewPage.userPasswordLabel.MaximumWidthRequest = v, 100, 300);
            animation.Commit(this, "ViewPage.userPasswordBorderWidthAnimation", 16, 300, Easing.CubicOut);
            animation2.Commit(this, "ViewPage.userPasswordLabelWidthAnimation", 16, 300, Easing.CubicOut);
            ViewPage.userPasswordLabel.FadeTo(1, 300, Easing.CubicOut);
            ViewPage.userPasswordBorder.FadeTo(1, 300, Easing.CubicOut);
            isPasswordChanged = true;
        }
    }

    internal void UserPasswordEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (ViewPage.userPasswordEditor.Text is null || ViewPage.userPasswordEditor.Text == string.Empty)
        {
            if (isPasswordChanged)
            {
                var animation = new Animation(v => ViewPage.userPasswordBorder.MaximumWidthRequest = v, 300, 100);
                var animation2 = new Animation(v => ViewPage.userPasswordLabel.MaximumWidthRequest = v, 300, 100);
                animation.Commit(this, "ViewPage.userPasswordBorderWidthAnimation", 16, 200, Easing.CubicOut);
                animation2.Commit(this, "ViewPage.userPasswordLabelWidthAnimation", 16, 200, Easing.CubicOut);
                ViewPage.userPasswordLabel.FadeTo(0.5, 300, Easing.CubicOut);
                ViewPage.userPasswordBorder.FadeTo(0.5, 200, Easing.CubicOut);
                isPasswordChanged = false;
            }
        }
    }

    internal void TelVerifyCodeEditor_Focused(object sender, FocusEventArgs e)
    {
        TelVerifyCodeLabel.FadeTo(1, 300, Easing.CubicOut);
        TelVerifyCodeLabel.ScaleTo(1.2, 300, Easing.CubicOut);
        TelVerifyCodeBorder.FadeTo(1, 300, Easing.CubicOut);
        TelVerifyCodeBorder.ScaleTo(1.2, 300, Easing.CubicOut);
        TelVerifyCodeButton.ScaleTo(0.8, 300, Easing.CubicOut);
    }

    internal void TelVerifyCodeEditor_Unfocused(object sender, FocusEventArgs e)
    {
        TelVerifyCodeLabel.FadeTo(0.5, 300, Easing.CubicOut);
        TelVerifyCodeLabel.ScaleTo(1, 300, Easing.CubicOut);
        TelVerifyCodeBorder.FadeTo(0.5, 300, Easing.CubicOut);
        TelVerifyCodeBorder.ScaleTo(1, 300, Easing.CubicOut);
        TelVerifyCodeButton.ScaleTo(1, 300, Easing.CubicOut);
    }

    internal void UserTelEditor_Focused(object sender, FocusEventArgs e)
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

    internal void UserTelEditor_Unfocused(object sender, FocusEventArgs e)
    {
        if (ViewPage.userTelEditor.Text is null || ViewPage.userTelEditor.Text == string.Empty)
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
    #endregion
    #region 编辑框文本改变事件
    internal void UserAccountEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserAccountEditor.Text))
        {
            ViewPage.userLoginButton.BackgroundColor = Color.FromArgb("#A491E8");
            if (!ViewPage.userLoginButton.IsWaiting)
                ViewPage.userLoginButton.IsEnabled = false;
            isAccountEditorEmpty = true;
        }
        else
        {
            if (isAccountEditorEmpty && !isPasswordEditorEmpty)
            {
                ViewPage.userLoginButton.BackgroundColor = Color.FromArgb("#512BD4");
                if (!ViewPage.userLoginButton.IsWaiting)
                    ViewPage.userLoginButton.IsEnabled = true;
            }
            isAccountEditorEmpty = false;
        }
    }

    internal void UserPasswordEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewPage.userPasswordEditor.Text))
        {
            ViewPage.userLoginButton.BackgroundColor = Color.FromArgb("#A491E8");
            if (!ViewPage.userLoginButton.IsWaiting)
                ViewPage.userLoginButton.IsEnabled = false;
            isPasswordEditorEmpty = true;
        }
        else
        {
            if (isPasswordEditorEmpty && !isAccountEditorEmpty)
            {
                ViewPage.userLoginButton.BackgroundColor = Color.FromArgb("#512BD4");
                if (!ViewPage.userLoginButton.IsWaiting)
                    ViewPage.userLoginButton.IsEnabled = true;
            }
            isPasswordEditorEmpty = false;
        }
    }

    internal void TelVerifyCodeEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!isTelEditorEmpty && TelVerifyCodeEditor.Text.Length == 6)
        {
            if (!ViewPage.userLoginButton.IsWaiting)
            {
                ViewPage.userLoginButton.BackgroundColor = Color.Parse("#512BD4");
                ViewPage.userLoginButton.IsEnabled = true;
            }
            TelVerifyCodeEditor_Unfocused(null, null);
        }
        else
        {
            if (!ViewPage.userLoginButton.IsWaiting)
            {
                ViewPage.userLoginButton.BackgroundColor = Color.Parse("#A491E8");
                ViewPage.userLoginButton.IsEnabled = false;
            }
        }
    }

    internal void UserTelEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ViewPage.userTelEditor.Text.IsMobPhoneNumber())
        {
            isTelEditorEmpty = false;
            UserTelLabel.Text = "手机号";
            UserTelLabel.TextColor = Color.Parse("Black");
            UserTelBorder.Stroke = Color.FromArgb("#444654");
            if (!isCoolDown)
            {
                TelVerifyCodeButton.IsEnabled = true;
                TelVerifyCodeButton.BackgroundColor = Color.FromArgb("#512BD4");
            }
        }
        else
        {
            isTelEditorEmpty = true;
            UserTelLabel.Text = "手机号格式不正确";
            UserTelLabel.TextColor = Color.Parse("Red");
            TelVerifyCodeButton.IsEnabled = false;
            TelVerifyCodeButton.BackgroundColor = Color.FromArgb("#A491E8");
        }
    }
    #endregion
    #region 按钮点击事件
    internal async void UserLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        try
        {
            switch (AppSystemProfile.LoginMethod)
            {
                case LoginMethod.PasswordLogin:
                    UserAccountBorder.Stroke = Color.FromArgb("#444654");
                    UserAccountLabel.Text = "手机号/邮箱";
                    UserAccountLabel.TextColor = Color.Parse("Gray");
                    ViewPage.userPasswordBorder.Stroke = Color.FromArgb("#444654");
                    ViewPage.userPasswordLabel.Text = "用户密码";
                    ViewPage.userPasswordLabel.TextColor = Color.Parse("Gray");
                    var mailResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Amail == UserAccountEditor.Text);
                    var telResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == UserAccountEditor.Text);
                    if (mailResult is not null && mailResult.Count > 0)
                        await ProcessLoginInfo(mailResult.FirstOrDefault(), e);
                    else if (telResult is not null && telResult.Count > 0)
                        await ProcessLoginInfo(telResult.FirstOrDefault(), e);
                    else
                    {
                        ControlExtension.BorderShake(UserAccountBorder);
                        UserAccountLabel.Text = "手机号或邮箱不存在";
                        UserAccountLabel.TextColor = Color.Parse("Red");
                        UserAccountBorder.Stroke = Color.Parse("Red");
                        UserAccountEditor.Focus();
                        e.Continue();
                    }
                    await new Action(() =>
                    {
                        Thread.Sleep(500);
                        e.Continue();
                    }).StartNewTask();
                    break;
                case LoginMethod.VerifyCodeLogin:
                    TelVerifyCodeBorder.Stroke = Color.FromArgb("#444654");
                    TelVerifyCodeLabel.TextColor = Color.Parse("Gray");
                    TelVerifyCodeLabel.Text = "验证码";
                    UserTelBorder.Stroke = Color.FromArgb("#444654");
                    UserTelLabel.TextColor = Color.Parse("Gray");
                    UserTelLabel.Text = "手机号";
                    if (TelVerifyCodeEditor.Text == randomCode)
                    {
                        if (ViewPage.userTelEditor.Text == currentPhoneNum)
                        {
                            var verifyTelResult = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.Atel == ViewPage.userTelEditor.Text);
                            if (verifyTelResult.Count > 0)
                            {
                                await ProcessLoginInfo(verifyTelResult.FirstOrDefault(), e);
                                return;
                            }
                            else
                            {
                                UserTelLabel.Text = "手机号不存在，请检查是否填写正确";
                                UserTelLabel.TextColor = Color.Parse("Red");
                                UserTelBorder.Stroke = Color.Parse("Red");
                                ControlExtension.BorderShake(UserTelBorder);
                                ViewPage.userTelEditor.Focus();
                                e.Continue();
                            }
                        }
                        else
                        {
                            UserTelLabel.TextColor = Color.Parse("Red");
                            UserTelBorder.Stroke = Color.Parse("Red");
                            UserTelLabel.Text = "手机号与验证码发送时的不一致";
                            ViewPage.userTelEditor.Focus();
                            ControlExtension.BorderShake(UserTelBorder);
                        }
                    }
                    else
                    {
                        TelVerifyCodeLabel.TextColor = Color.Parse("Red");
                        TelVerifyCodeBorder.Stroke = Color.Parse("Red");
                        TelVerifyCodeLabel.Text = "验证码错误";
                        TelVerifyCodeEditor.Focus();
                        ControlExtension.BorderShake(TelVerifyCodeBorder);
                    }
                    break;

                default:
                    ProcessException.ShowEnumException();
                    break;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current?.DisplayAlert("登录时错误", ex.Message, "确认");
        }
    }

    internal async void SwitchToRegisterPageButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(UserRegisterPage));
        e.Continue();
    }

    internal async void SwitchToTelVerifyCodeLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        await SwitchToTelVerifyCodeLoginStyle();
        e.Continue();
    }

    internal async void SwitchToPasswordLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e)
    {
        AppSystemProfile.LoginMethod = LoginMethod.PasswordLogin;
        AppSystemProfile.SaveSystemProfile();
        #region FadeAnimation
        _ = UserTelLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
        _ = UserTelBorder.TranslateTo(-100, 0, 800, Easing.SpringOut);
        _ = TelVerifyCodeLabel.TranslateTo(-100, 0, 800, Easing.SpringOut);
        _ = TelVerifyCodeGrid.TranslateTo(-100, 0, 800, Easing.SpringOut);
        _ = UserTelLabel.FadeTo(0, 800, Easing.SpringOut);
        _ = UserTelBorder.FadeTo(0, 800, Easing.SpringOut);
        _ = TelVerifyCodeLabel.FadeTo(0, 800, Easing.SpringOut);
        _ = TelVerifyCodeGrid.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.switchToPasswordLoginButton.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.userLoginButton.FadeTo(0, 800, Easing.SpringOut);
        _ = forgotPasswordButton.FadeTo(0, 800, Easing.SpringOut);
        await ViewPage.switchToRegisterPageButton.FadeTo(0, 800, Easing.SpringOut);
        #endregion
        #region SetInvisible
        UserTelLabel.IsVisible = false;
        UserTelBorder.IsVisible = false;
        TelVerifyCodeLabel.IsVisible = false;
        TelVerifyCodeGrid.IsVisible = false;
        ViewPage.switchToPasswordLoginButton.IsVisible = false;
        #endregion
        #region SetVisible
        UserAccountLabel.TranslationX = 0;
        UserAccountLabel.TranslationY = 150;
        UserAccountLabel.IsVisible = true;
        UserAccountBorder.TranslationX = 0;
        UserAccountBorder.TranslationY = 150;
        UserAccountBorder.IsVisible = true;
        ViewPage.userPasswordLabel.TranslationX = 0;
        ViewPage.userPasswordLabel.TranslationY = 110;
        ViewPage.userPasswordLabel.IsVisible = true;
        ViewPage.userPasswordBorder.TranslationX = 0;
        ViewPage.userPasswordBorder.TranslationY = 110;
        ViewPage.userPasswordBorder.IsVisible = true;
        forgotPasswordButton.TranslationX = 0;
        forgotPasswordButton.TranslationY = 70;
        ViewPage.userLoginButton.TranslationX = 0;
        ViewPage.userLoginButton.TranslationY = 50;
        SwitchToTelVerifyCodeLoginButton.TranslationX = 0;
        SwitchToTelVerifyCodeLoginButton.TranslationY = 30;
        SwitchToTelVerifyCodeLoginButton.IsVisible = true;
        ViewPage.switchToRegisterPageButton.TranslationX = 0;
        ViewPage.switchToRegisterPageButton.TranslationY = 20;
        #endregion
        #region ShowAnimation
        await new Action(() =>
        {
            ViewPage.Dispatcher.Dispatch(() =>
            {
                UserAccountLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                UserAccountBorder.FadeTo(0.5, 1000, Easing.CubicOut);
                UserAccountLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                UserAccountBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.userPasswordLabel.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.userPasswordLabel.FadeTo(0.5, 1000, Easing.CubicOut);
                ViewPage.userPasswordBorder.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.userPasswordBorder.FadeTo(0.5, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                forgotPasswordButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                forgotPasswordButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.userLoginButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.userLoginButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                SwitchToTelVerifyCodeLoginButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                SwitchToTelVerifyCodeLoginButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.switchToRegisterPageButton.TranslateTo(0, 0, 1000, Easing.CubicOut);
                ViewPage.switchToRegisterPageButton.FadeTo(1, 1000, Easing.CubicOut);
            });
            Thread.Sleep(200);
        }).StartNewTask();
        UserAccountEditor.Focus();
        e.Continue();
        #endregion
    }

    [RelayCommand]
    private async Task SendVerifyCode()
    {
        currentPhoneNum = ViewPage.userTelEditor.Text;
        randomCode = IDGenerator.SummonRandomID(6);
        TelVerifyCodeButton.IsEnabled = false;
        TelVerifyCodeButton.BackgroundColor = Color.FromArgb("#A491E8");
        TelVerifyCodeButton.Text = "发送中...";
        isCoolDown = true;
        var resp = await TencentSms.SendVerifyCode("1922756", "+86" + ViewPage.userTelEditor.Text, [randomCode, "2"]);
        if (resp == null || resp.SendStatusSet.First().Code != "Ok")
        {
            await Shell.Current?.DisplayAlert("出错啦！", $"验证码发送失败：{resp?.SendStatusSet.First().Message}\n手机号：{ViewPage.userTelEditor.Text}", "啊？");
            TelVerifyCodeButton.IsEnabled = true;
            TelVerifyCodeButton.BackgroundColor = Color.FromArgb("#512BD4");
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
                        TelVerifyCodeButton.BackgroundColor = Color.FromArgb("#512BD4");
                        return false;
                    }
                    return true;
                });
            }).StartNewTask();
        }
    }

    [RelayCommand]
    private void ForgotPassword() => Shell.Current.GoToAsync(nameof(ForgetPasswordPage));
    #endregion
}
