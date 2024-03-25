using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class AICenterPage : ContentPage
{
    public AICenterPageViewModel ViewModel { get; init; }
    public AICenterPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }
}