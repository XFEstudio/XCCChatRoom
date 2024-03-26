using System.Diagnostics;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.ViewModel;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.TaskExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewPage;

public partial class UserLoginPage : ContentPage
{
    internal UserLoginPageViewModel ViewModel { get; init; }
    public UserLoginPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (UserAccountEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (UserPasswordEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (UserTelEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (TelVerifyCodeEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    #region 焦点事件
    private void UserAccountEditor_Focused(object sender, FocusEventArgs e) => ViewModel.UserAccountEditor_Focused(sender, e);

    private void UserAccountEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.UserAccountEditor_Unfocused(sender, e);

    private void UserPasswordEditor_Focused(object sender, FocusEventArgs e) => ViewModel.UserPasswordEditor_Focused(sender, e);

    private void UserPasswordEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.UserPasswordEditor_Unfocused(sender, e);

    private void TelVerifyCodeEditor_Focused(object sender, FocusEventArgs e) => ViewModel.TelVerifyCodeEditor_Focused(sender, e);

    private void TelVerifyCodeEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.TelVerifyCodeEditor_Unfocused(sender, e);
    private void UserTelEditor_Focused(object sender, FocusEventArgs e) => ViewModel.UserTelEditor_Focused(sender, e);

    private void UserTelEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.UserTelEditor_Unfocused(sender, e);
    #endregion
    #region 编辑框文本改变事件
    private void UserAccountEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.UserAccountEditor_TextChanged(sender, e);

    private void UserPasswordEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.UserPasswordEditor_TextChanged(sender, e);

    private void TelVerifyCodeEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.TelVerifyCodeEditor_TextChanged(sender, e);

    private void UserTelEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.UserTelEditor_TextChanged(sender, e);
    #endregion
    #region 点击事件
    private void SwitchToPasswordLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e) => ViewModel.SwitchToPasswordLoginButton_WaitClick(sender, e);
    private void SwitchToTelVerifyCodeLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e) => ViewModel.SwitchToTelVerifyCodeLoginButton_WaitClick(sender, e);
    private void SwitchToRegisterPageButton_WaitClick(object sender, WaitButtonClickedEventArgs e) => ViewModel.SwitchToRegisterPageButton_WaitClick(sender, e);
    private void UserLoginButton_WaitClick(object sender, WaitButtonClickedEventArgs e) => ViewModel.UserLoginButton_WaitClick(sender, e);
    #endregion
}