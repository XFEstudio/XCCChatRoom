using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class IntroductionPage : ContentPage
{
    public IntroductionPageViewModel ViewModel { get; init; }

    public IntroductionPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }
}