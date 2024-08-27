using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using XCCChatRoom.AllImpl;
using XFEExtension.NetCore.ProfileExtension;
using XFEExtension.NetCore.StringExtension;

namespace XCCChatRoom;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        XFEProfile.ProfilesRootPath = AppPath.ProfilesPath;
        AppPath.AppChatImageSavePath.XL();
        XCCDataBase.Initialize();
        TencentSms.Initialize();
        GPTAIDialogManager.LoadDialogs();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if WINDOWS
                handlers.AddHandler(typeof(Entry), typeof(Platforms.Windows.WinEntryHandler));
                handlers.AddHandler(typeof(Editor), typeof(Platforms.Windows.WinEditorHandler));
#endif
            })
#if ANDROID
            .UseLocalNotification()
#endif
            .UseMauiCommunityToolkit();

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