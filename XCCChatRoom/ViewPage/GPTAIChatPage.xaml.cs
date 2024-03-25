using XCCChatRoom.AllImpl;
using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class GPTAIChatPage : ContentPage
{
    public GPTAIChatPageViewModel ViewModel { get; init; }
    public static GPTAIChatPage Current { get; private set; }
    public GPTAIChatPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
        Current = this;
    }

    protected override bool OnBackButtonPressed()
    {
        GPTAIDialogManager.SaveDialogs();
        return base.OnBackButtonPressed();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (inputEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    private void InputEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel.InputEditor_TextChanged(sender, e);
    }

    private void ChatScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        ViewModel.ChatScrollView_Scrolled(sender, e);
    }
}
class LabelAndMessageId
{
    public Label label;
    public string messageId;
    public bool completeState = false;
    public LabelAndMessageId(Label label, string messageId)
    {
        this.label = label;
        this.messageId = messageId;
    }
    public LabelAndMessageId(string messageId)
    {
        this.messageId = messageId;
    }
}