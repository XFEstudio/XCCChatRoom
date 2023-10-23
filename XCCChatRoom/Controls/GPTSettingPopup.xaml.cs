using MauiPopup;
using MauiPopup.Views;
using XCCChatRoom.InnerPage;

namespace XCCChatRoom.Controls;

public partial class GPTSettingPopup : BasePopupPage
{
    public GPTSettingPopup()
    {
        InitializeComponent();
        TitleEditor.Text = GPTAIChatPage.Current.DialogTitle;
        SystemEditor.Text = GPTAIChatPage.Current.System;
        GPTVersionLabel.Text = GPTAIChatPage.Current.GPTVersion;
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
        GPTAIChatPage.Current.DialogTitle = TitleEditor.Text;
        GPTAIChatPage.Current.System = SystemEditor.Text;
        await PopupAction.ClosePopup();
    }
}