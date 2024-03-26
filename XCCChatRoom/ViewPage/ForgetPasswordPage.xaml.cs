using XCCChatRoom.ViewModel;
namespace XCCChatRoom.ViewPage;

public partial class ForgetPasswordPage : ContentPage
{
	internal ForgetPasswordPageViewModel ViewModel { get; init; }

    public ForgetPasswordPage()
	{
		InitializeComponent();
		ViewModel = new(this);
		BindingContext = ViewModel;
	}
}