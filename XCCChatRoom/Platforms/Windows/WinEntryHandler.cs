using Microsoft.UI.Xaml.Controls;

namespace XCCChatRoom.Platforms.Windows;

public class WinEntryHandler : Microsoft.Maui.Handlers.EntryHandler
{
    protected override void ConnectHandler(TextBox platformView)
    {
        platformView.Background = null;
        base.ConnectHandler(platformView);
    }
}
