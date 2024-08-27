using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class XEAEncryptPage : ContentPage
{
    internal XEAEncryptPageViewModel ViewModel { get; init; }
    public XEAEncryptPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }
}