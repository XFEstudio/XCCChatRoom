using MauiPopup;
using MauiPopup.Views;
using Timer = System.Timers.Timer;

namespace XCCChatRoom.Controls;

public partial class TipPopup : BasePopupPage
{
    public TipPopup(string title, double closeTime = 3)
    {
        InitializeComponent();
        this.Shadow = new Shadow
        {
            Opacity = 0.5f,
            Brush = Color.FromArgb("#512BD4"),
            Radius = 5,
            Offset = new Point(4, 4),
        };
        DisplayTitleLabel.Text = title;
        DisplayContentLabel.IsVisible = false;
        CloseTimer(closeTime);
    }
    public TipPopup(string title, string content, double closeTime = 3)
    {
        InitializeComponent();
        this.Shadow = new Shadow
        {
            Opacity = 0.5f,
            Brush = Color.FromArgb("#512BD4"),
            Radius = 5,
            Offset = new Point(4, 4),
        };
        DisplayTitleLabel.Text = title;
        DisplayContentLabel.Text = content;
        CloseTimer(closeTime);
    }
    private void CloseTimer(double second)
    {
        Timer timer = new Timer(second * 1000);
        timer.Elapsed += (sender, e) =>
        {
            this.Dispatcher.Dispatch(async () =>
            {
                await PopupAction.ClosePopup();
            });
        };
        timer.Start();
    }
}