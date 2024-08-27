using Microsoft.UI.Xaml.Controls;

namespace XCCChatRoom.Platforms.Windows;

public class WinEditorHandler : Microsoft.Maui.Handlers.EditorHandler
{
    protected override void ConnectHandler(TextBox platformView)
    {
        platformView.Background = null;
        base.ConnectHandler(platformView);
    }
}
