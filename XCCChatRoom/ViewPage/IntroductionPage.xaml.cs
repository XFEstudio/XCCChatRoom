using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class IntroductionPage : ContentPage
{
    internal IntroductionPageViewModel ViewModel { get; init; }

    public IntroductionPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }

    private void SkipButton_Click(object sender, TappedEventArgs e) => ViewModel.SkipPage(sender, e);
}