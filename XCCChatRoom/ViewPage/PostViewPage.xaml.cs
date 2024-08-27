using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

[QueryProperty(nameof(PostID), nameof(PostID))]
public partial class PostViewPage : ContentPage
{
    internal PostViewPageViewModel ViewModel { get; init; }
    private string postID;

    public string PostID
    {
        get { return postID; }
        set
        {
            if (string.IsNullOrEmpty(value))
                return;
            postID = value;
            if (!ViewModel.initialized)
                ViewModel.InitializePageData();
        }
    }
    public static PostViewPage Current { get; private set; }
    public PostViewPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
        Current = this;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (inputEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    protected override bool OnBackButtonPressed()
    {
        try { ViewModel.XFEExecuter.Dispose(); } catch (Exception) { }
        ViewModel.CurrentPostData = null;
        return base.OnBackButtonPressed();
    }

    private void InputEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.InputEditor_TextChanged(sender, e);

    private void LikeButton_Clicked(object sender, EventArgs e) => ViewModel.LikeButton_Clicked(sender, e);

    private void StarButton_Clicked(object sender, EventArgs e) => ViewModel.StarButton_Clicked(sender, e);
}