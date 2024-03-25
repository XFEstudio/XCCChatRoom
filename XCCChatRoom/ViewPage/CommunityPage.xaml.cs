using System.Diagnostics;
using XCCChatRoom.AllImpl;
using XCCChatRoom.ViewModel;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewPage;

public partial class CommunityPage : ContentPage
{
    public static CommunityPage Current { get; private set; }
    public CommunityPageViewModel ViewModel { get; init; }
    public CommunityPage()
    {
        InitializeComponent();
        Current = this;
        postRefreshView.IsRefreshing = true;
    }

    private void PostScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        if (RefreshingIsBusy)
            return;
        if (totalHeight - e.ScrollY - postScrollView.Height <= 0)
        {
            RefreshingIsBusy = true;
            GetDownPost();
            Trace.WriteLine("加载更多");
        }
    }
}