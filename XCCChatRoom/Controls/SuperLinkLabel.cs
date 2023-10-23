namespace XCCChatRoom.Controls;

public partial class SuperLinkLabel : Label
{
    public Color ClickedColor { get; set; } = Color.FromArgb("#444654");
    public bool EnableClick { get; set; } = true;
    public bool Underline { get; set; } = true;
    public event EventHandler<TappedEventArgs> Click;
    public SuperLinkLabel()
    {
        if (Underline)
        {
            this.TextDecorations = TextDecorations.Underline;
        }
        var recognizer = new TapGestureRecognizer();
        recognizer.Tapped += TapGestureRecognizer_Tapped;
        this.GestureRecognizers.Add(recognizer);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (EnableClick)
        {
            await this.ScaleTo(0.8, 50, Easing.CubicOut);
            this.TextColor = ClickedColor;
            Click?.Invoke(this, e);
            await this.ScaleTo(1, 50, Easing.CubicOut);
        }
    }
}