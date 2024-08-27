using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class XAPPViewPage : ContentPage
{
    internal XAPPViewPageViewModel ViewModel { get; init; }
    public XAPPViewPage()
    {
        InitializeComponent();
        ViewModel = new XAPPViewPageViewModel(this);
        BindingContext = ViewModel;
        listView.BeginRefresh();
    }

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e) => ViewModel.ListView_ItemTapped(sender, e);
}