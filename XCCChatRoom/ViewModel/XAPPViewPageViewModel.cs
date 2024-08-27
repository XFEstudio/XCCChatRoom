using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XCCChatRoom.Model;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.FormatExtension;
using XFEExtension.NetCore.WebExtension;
using XFEExtension.NetCore.XApp.Core;

namespace XCCChatRoom.ViewModel;

internal partial class XAPPViewPageViewModel(XAPPViewPage viewPage) : ObservableObject
{
    [ObservableProperty]
    List<XAppInformationView> xAPPInformationList = [];
    public XAPPViewPage ViewPage { get; init; } = viewPage;

    internal void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is XAppInformationView appInfo)
        {
            Application.Current!.OpenWindow(new Window(new XAppLoadPage(appInfo))
            {
                Title = appInfo.AppName
            });
        }
    }

    [RelayCommand]
    async Task RefreshList()
    {
        try
        {
            var list = new List<XAppInformationView>();
            var result = await "XAppGetUrl".GetFromURLAsync();
            if (result is not null)
            {
                XFEDictionary appInfoListDictionary = result;
                foreach (var appInfoEntry in appInfoListDictionary)
                    list.Add(XAppInformationView.LoadFromXAppInformation(XAppInformation.UnPackage(appInfoEntry.Content)));
                XAPPInformationList = list;
            }
            else
            {
#if ANDROID
                await Toast.Make("无法获取").Show();
#else
                await Shell.Current.DisplayAlert("无法获取", "服务器无返回内容", "确认");
#endif
            }
        }
        catch (Exception ex)
        {
#if ANDROID
            await Toast.Make($"无法获取：{ex.Message}").Show();
#else
            await Shell.Current.DisplayAlert("无法获取", ex.Message, "确认");
#endif
        }
        ViewPage.listView.Dispatcher.Dispatch(() => ViewPage.listView.IsRefreshing = false);
    }
}
