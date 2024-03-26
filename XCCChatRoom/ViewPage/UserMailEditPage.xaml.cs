using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class UserMailEditPage : ContentPage
{
    internal UserMailEditPageViewModel ViewModel { get; init; }
    public UserMailEditPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }
    private void NewMailEditor_Unfocused(object sender, FocusEventArgs e) => ViewModel.NewMailEditor_Unfocused(sender, e);

    private void MailEditorCaptcha_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.MailEditorCaptcha_TextChanged(sender, e);
}