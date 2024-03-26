using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class UserPrivacyListPage : ContentPage
{
	internal UserPrivacyListPageViewModel ViewModel { get; init; }
	public UserPrivacyListPage()
	{
        InitializeComponent();
		ViewModel = new(this);
		BindingContext = ViewModel;
	}
}