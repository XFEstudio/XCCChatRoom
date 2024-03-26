using XCCChatRoom.AllImpl;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.FileExtension;

namespace XCCChatRoom;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        try
        {
            if (File.Exists(AppPath.CheckInitializePath))
            {
                MainPage = new AppShell();
            }
            else
            {
                "1".WriteIn(AppPath.CheckInitializePath);
                MainPage = new IntroductionPage();
            }
        }
        catch
        {
            MainPage = new AppShell();
        }
    }
}
