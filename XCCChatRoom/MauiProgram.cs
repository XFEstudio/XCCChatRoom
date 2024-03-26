using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using XCCChatRoom.AllImpl;
using XFEExtension.NetCore.ProfileExtension;

namespace XCCChatRoom;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        XFEProfile.ProfilesRootPath = AppPath.ProfilesPath;
        AppSystemProfile.LoadSystemProfile();
        XCCDataBase.Initialize();
        TencentSms.Initialize();
        GPTAIDialogManager.LoadDialogs();
#if ANDROID
        builder.UseMauiApp<App>().UseLocalNotification().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkit();
#endif
#if WINDOWS
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkit();
#endif
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}