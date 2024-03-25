using System.Diagnostics;
using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class CommunityPage : ContentPage
{
    public static CommunityPage Current { get; private set; }
    public CommunityPageViewModel ViewModel { get; init; }
    public CommunityPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
        Current = this;
        postRefreshView.IsRefreshing = true;
    }

    private void PostScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        if (ViewModel.RefreshingIsBusy)
            return;
        if (ViewModel.totalHeight - e.ScrollY - postScrollView.Height <= 0)
        {
            ViewModel.RefreshingIsBusy = true;
            ViewModel.GetDownPost();
            Trace.WriteLine("¼ÓÔØ¸ü¶à");
        }
    }
}