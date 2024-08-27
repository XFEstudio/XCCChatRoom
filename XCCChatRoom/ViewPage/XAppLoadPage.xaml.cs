using XCCChatRoom.Model;
using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class XAppLoadPage : ContentPage
{
    internal XAppLoadPageViewModel ViewModel { get; init; }
    public XAppLoadPage(XAppInformationView appInfo)
    {
        this.Loaded += XAppLoadPage_Loaded;
        InitializeComponent();
        ViewModel = new(this, appInfo);
        BindingContext = ViewModel;
        Title = appInfo.AppName;
        Task.Run(ViewModel.XAppLoadingAnimation);
    }

    private async void XAppLoadPage_Loaded(object? sender, EventArgs e) => await Task.Run(ViewModel.LoadXApp);
}