using XCCChatRoom.AllImpl;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class UserPasswordEditorPage : ContentPage
{
    internal UserPasswordEditorPageViewModel ViewModel { get; init; }
    public UserPasswordEditorPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }

    private void OriginPassword_Unfocused(object sender, FocusEventArgs e) => ViewModel.OriginPassword_Unfocused(sender, e);

    private void NewPassword_Unfocused(object sender, FocusEventArgs e) => ViewModel.NewPassword_Unfocused(sender, e);

    private void NewPasswordConfirmation_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.NewPasswordConfirmation_TextChanged(sender, e);

    private void ForgotPasswordButton_Click(object sender, TappedEventArgs e) => ViewModel.ForgotPasswordButton_Click(sender, e);
}