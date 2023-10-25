using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using XCCChatRoom.AllImpl;

namespace XCCChatRoom;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        AppSystemProfile.LoadSystemProfile();
        XCCDataBase.Initialize();
        TencentSms.Initialize();
        GPTAIDialogManager.LoadDialogs();
#if ANDROID
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
#endif
#if WINDOWS
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
