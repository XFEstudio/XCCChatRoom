using MauiPopup;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.ViewModel;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.TaskExtension;
using XFE¸÷ÀàÍØÕ¹.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewPage;

public partial class UserRegisterPage : ContentPage
{
    internal UserRegisterPageViewModel ViewModel { get; init; }
    public UserRegisterPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (userNameEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (userPasswordEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (userTelEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (userMailEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (userPasswordEnsureEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (telVerifyCodeEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    private void NextStepButton_WaitClick(object sender, WaitButtonClickedEventArgs e) => ViewModel.NextStepButton_WaitClick(sender, e);

    private void TelVerifyCodeButton_Clicked(object sender, EventArgs e) => ViewModel.TelVerifyCodeButton_Clicked(sender, e);

    private void UserRegisterButton_WaitClick(object sender, WaitButtonClickedEventArgs e) => ViewModel.UserRegisterButton_WaitClick(sender, e);

    private void SwitchToLoginPageButton_WaitClick(object sender, WaitButtonClickedEventArgs e) => ViewModel.SwitchToLoginPageButton_WaitClick(sender, e);
    #region ±à¼­¿òÄÚÈÝ¼ì²â
    private void UserTelEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.UserTelEditor_TextChanged(sender, e);

    private void UserMailEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.UserMailEditor_TextChanged(sender, e);

    private void UserNameEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.UserNameEditor_TextChanged(sender, e);

    private void UserPasswordEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.UserPasswordEditor_TextChanged(sender, e);

    private void UserPasswordEnsureEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.UserPasswordEnsureEditor_TextChanged(sender, e);

    private void TelVerifyCodeEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.TelVerifyCodeEditor_TextChanged(sender, e);
    #endregion
    #region ±à¼­¿ò½¹µãÊÂ¼þ
    private void UserTelEditor_Focused(object sender, FocusEventArgs e) => ViewModel.UserTelEditor_Focused(sender, e);

    private void UserTelEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.UserTelEditor_Unfocused(sender, e);

    private void UserMailEditor_Focused(object sender, FocusEventArgs e) => ViewModel.UserMailEditor_Focused(sender, e);

    private void UserMailEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.UserMailEditor_Unfocused(sender, e);

    private void UserNameEditor_Focused(object sender, FocusEventArgs e) => ViewModel.UserNameEditor_Focused(sender, e);

    private void UserNameEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.UserNameEditor_Unfocused(sender, e);

    private void TelVerifyCodeEditor_Focused(object sender, FocusEventArgs e) => ViewModel.TelVerifyCodeEditor_Focused(sender, e);

    private void TelVerifyCodeEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.TelVerifyCodeEditor_Unfocused(sender, e);

    private void UserPasswordEditor_Focused(object sender, FocusEventArgs e) => ViewModel.UserPasswordEditor_Focused(sender, e);

    private void UserPasswordEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.UserPasswordEditor_Unfocused(sender, e);

    private void UserPasswordEnsureEditor_Focused(object sender, FocusEventArgs e) => ViewModel.UserPasswordEnsureEditor_Focused(sender, e);

    private void UserPasswordEnsureEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.UserPasswordEnsureEditor_Unfocused(sender, e);
    #endregion
}