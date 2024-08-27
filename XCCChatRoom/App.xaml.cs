using XCCChatRoom.AllImpl;
using XCCChatRoom.Profiles;
using XCCChatRoom.Resources.Theme;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom;

public partial class App : Application
{
    public App()
    {
        Current!.RequestedThemeChanged += (sender, e) =>
        {
            if (Current!.UserAppTheme != AppTheme.Unspecified)
                SystemProfile.Theme = e.RequestedTheme;
            AutoSwitchByTheme(e.RequestedTheme);
            ThemeImageGetter.ThemeChanged(Current!.RequestedTheme);
        };
        InitializeComponent();
        try
        {
            if (VersionTracking.IsFirstLaunchEver)
                MainPage = new IntroductionPage();
            else
                MainPage = new AppShell();
        }
        catch
        {
            MainPage = new AppShell();
        }
        SystemProfile.Theme = Current!.RequestedTheme;
        Console.WriteLine(Current!.RequestedTheme);
        AutoSwitchByTheme(SystemProfile.Theme);
    }

    public static void SwitchToTheme(ResourceDictionary resourceDictionary)
    {
        if (Current is not null)
        {
            Current.Resources.MergedDictionaries.Clear();
            Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }

    public static void AutoSwitchByTheme(AppTheme appTheme)
    {
        switch (appTheme)
        {
            case AppTheme.Unspecified:
                switch (Current!.RequestedTheme)
                {
                    case AppTheme.Unspecified:
                        SwitchToTheme(new LightTheme());
                        break;
                    case AppTheme.Light:
                        SwitchToTheme(new LightTheme());
                        break;
                    case AppTheme.Dark:
                        SwitchToTheme(new DarkTheme());
                        break;
                    default:
                        break;
                }
                break;
            case AppTheme.Light:
                SwitchToTheme(new LightTheme());
                break;
            case AppTheme.Dark:
                SwitchToTheme(new DarkTheme());
                break;
            default:
                break;
        }
    }
}
