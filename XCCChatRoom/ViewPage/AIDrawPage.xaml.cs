using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class AIDrawPage : ContentPage
{
	public AIDrawPageViewModel ViewModel { get; init; }
	public AIDrawPage()
	{
		InitializeComponent();
		ViewModel = new(this);
		BindingContext = ViewModel;
	}
}