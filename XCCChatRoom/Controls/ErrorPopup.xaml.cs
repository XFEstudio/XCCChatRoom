using MauiPopup;
using MauiPopup.Views;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.Controls;

public partial class ErrorPopup : BasePopupPage
{
    public ErrorPopup(string title)
    {
        InitializeComponent();
        this.Shadow = new Shadow
        {
            Opacity = 0.5f,
            Brush = Color.FromArgb("#ea4f4f"),
            Radius = 5,
            Offset = new Point(4, 4),
        };
        DisplayTitleLabel.Text = title;
        DisplayContentLabel.IsVisible = false;
        CloseTimer(3);
    }
    public ErrorPopup(string title, string content, int closeTime = 3)
    {
        InitializeComponent();
        DisplayTitleLabel.Text = title;
        DisplayContentLabel.Text = content;
        CloseTimer(closeTime);
    }
    private void CloseTimer(int second)
    {
        Timer timer = new Timer(second * 1000);
        timer.Elapsed += (sender, e) =>
        {
            this.Dispatcher.Dispatch(async () =>
            {
                timer.Close();
                await PopupAction.ClosePopup();
            });
        };
        timer.Start();
    }
}