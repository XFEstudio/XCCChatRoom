using MauiPopup.Views;

namespace XCCChatRoom.Controls;

public partial class UpdatePopup : BasePopupPage
{
    public event EventHandler UpdateButtonClicked;
    public event EventHandler CancelButtonClicked;
    public event EventHandler SkipButtonClicked;
    public UpdatePopup(string updateTitle, string updateVersion, string updateContent, bool cancelable = true)
    {
        InitializeComponent();
        if (!cancelable)
        {
            CancelButton.IsVisible = false;
            SkipButton.IsVisible = false;
            IsCloseOnBackgroundClick = false;
        }
        DisplayTitleLabel.Text = updateTitle;
        DisplayVersionLabel.Text = updateVersion;
        DisplayContentLabel.Text = updateContent;
    }

    private void UpdateButton_Clicked(object sender, EventArgs e)
    {
        UpdateButtonClicked?.Invoke(this, e);
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        CancelButtonClicked?.Invoke(this, e);
    }

    private void SkipButton_Click(object sender, TappedEventArgs e)
    {
        SkipButtonClicked?.Invoke(this, e);
    }
}