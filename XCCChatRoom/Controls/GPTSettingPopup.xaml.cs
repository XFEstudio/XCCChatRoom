using MauiPopup;
using MauiPopup.Views;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom.Controls;

public partial class GPTSettingPopup : BasePopupPage
{
    public GPTSettingPopup()
    {
        InitializeComponent();
        TitleEditor.Text = GPTAIChatPage.Current.ViewModel.DialogTitle;
        SystemEditor.Text = GPTAIChatPage.Current.ViewModel.System;
        GPTVersionLabel.Text = GPTAIChatPage.Current.ViewModel.GPTVersion;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (TitleEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (SystemEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    private async void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        GPTAIChatPage.Current.ViewModel.DialogTitle = TitleEditor.Text;
        GPTAIChatPage.Current.ViewModel.System = SystemEditor.Text;
        await PopupAction.ClosePopup();
    }
}