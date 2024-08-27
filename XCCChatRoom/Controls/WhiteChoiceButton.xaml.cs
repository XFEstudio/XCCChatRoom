namespace XCCChatRoom.Controls;

public partial class WhiteChoiceButton : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(WhiteChoiceButton), string.Empty);
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(WhiteChoiceButton), 16.0);
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    public event EventHandler<TappedEventArgs> Click;
    public WhiteChoiceButton()
    {
        InitializeComponent();
        this.BindingContext = this;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var animation = new Animation(v => ButtonGrid.Background = Color.FromRgba(v, v, v, 255), 255, 160);
        var animation2 = new Animation(v => ButtonGrid.Background = Color.FromRgba(v, v, v, 255), 160, 255);
        animation.Commit(this, "TextLabelColorHighAnimation", 16, 200, Easing.CubicOut);
        await TextLabel.ScaleTo(0.9, 100, Easing.CubicOut);
        Click?.Invoke(this, e);
        animation2.Commit(this, "TextLabelColorLowAnimation", 16, 50, Easing.CubicOut);
        await TextLabel.ScaleTo(1, 100, Easing.CubicIn);
    }
}