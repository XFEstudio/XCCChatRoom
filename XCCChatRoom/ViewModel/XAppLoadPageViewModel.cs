#if ANDROID
using Android.OS;
#endif
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using XCCChatRoom.Model;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.WebExtension;
using XFEExtension.NetCore.XApp.Core;
using XFEExtension.NetCore.XApp.Extensions;

namespace XCCChatRoom.ViewModel;

internal class XAppLoadPageViewModel(XAppLoadPage viewPage, XAppInformationView appInfo) : ObservableObject
{
    public XAppInformationView AppInfo { get; init; } = appInfo;
    public XAppLoadPage ViewPage { get; init; } = viewPage;
    public bool IsAppLoaded { get; set; } = false;

    public async Task XAppLoadingAnimation()
    {
        while (!IsAppLoaded)
        {
            await ViewPage.loadCircle.Dispatcher.DispatchAsync(async () =>
            {
                await ViewPage.loadCircle.RotateTo(360, 1000, Easing.SinInOut);
                ViewPage.loadCircle.Rotation = 0;
            });

        }
    }

    public async Task LoadXApp()
    {
        try
        {
            var result = await "XAppGetUrl".GetFromURLAsync();
            if (result is not null)
            {
                var app = XApp.UnPackage(result);
                if (app is not null && app.AssemblyBytes is not null)
                {
                    var assembly = Assembly.Load(app.AssemblyBytes);
                    var compilateResult = await assembly.GetMainPage();
                    if (compilateResult.Page is not null)
                    {
                        IsAppLoaded = true;
                        await Task.Delay(500);
                        ViewPage.Window.Dispatcher.Dispatch(() => ViewPage.Window.Page = compilateResult.Page);
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
#if ANDROID
            Looper.Prepare();
            await Toast.Make($"应用打开失败：{ex.Message}").Show();
#else
            await ViewPage.DisplayAlert("无法打开该应用", $"应用无法打开{ex.Message}", "确认");
#endif
            IsAppLoaded = true;
            Application.Current!.CloseWindow(ViewPage.Window);
            return;
        }
#if ANDROID
        Looper.Prepare();
        await Toast.Make("应用打开失败").Show();
#else
        await ViewPage.DisplayAlert("无法打开该应用", "应用无法打开", "确认");
#endif
        IsAppLoaded = true;
        Application.Current!.CloseWindow(ViewPage.Window);
    }
}
