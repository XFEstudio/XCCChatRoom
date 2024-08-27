using MauiPopup.Views;
using XCCChatRoom.Controls.ViewModel;
using XCCChatRoom.Profiles;

namespace XCCChatRoom.Controls;

public partial class UpdatePopup : BasePopupPage
{
    internal UpdatePopupViewModel ViewModel { get; init; }
    public UpdatePopup(string updateTitle, string updateVersion, string updateContent, bool cancelable = true, string lastForceDownloadVersion = "")
    {
        InitializeComponent();
        ViewModel = new(this, updateTitle, updateVersion, updateContent, cancelable, lastForceDownloadVersion);
        BindingContext = ViewModel;
    }

    private void SkipButton_Click(object sender, TappedEventArgs e) => SystemProfile.IgnoreVersion = ViewModel.VersionText;
}