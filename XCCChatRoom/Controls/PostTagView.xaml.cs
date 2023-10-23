namespace XCCChatRoom.Controls;

public partial class PostTagView : ContentView
{
    public static readonly BindableProperty TagTextProperty = BindableProperty.Create(nameof(TagText), typeof(string), typeof(PostCardView), "TagText");
    public string TagText
    {
        get => (string)GetValue(TagTextProperty);
        set => SetValue(TagTextProperty, value);
    }
    public event EventHandler<PostTagViewTagClickEventArgs> Clicked;

    public PostTagView()
    {
        InitializeComponent();
        this.BindingContext = this;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Clicked?.Invoke(sender, new PostTagViewTagClickEventArgs(TagText, e));
    }
}
public class PostTagViewTagClickEventArgs : EventArgs
{
    public string TagText { get; init; }
    public TappedEventArgs TappedEventArgs { get; init; }
    public PostTagViewTagClickEventArgs(string tagText, TappedEventArgs e)
    {
        TagText = tagText;
        TappedEventArgs = e;
    }
}