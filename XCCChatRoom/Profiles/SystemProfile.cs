using XCCChatRoom.AllImpl;
using XFEExtension.NetCore.ProfileExtension;

namespace XCCChatRoom.Profiles;

public static partial class SystemProfile
{
    [ProfileProperty]
    private static AppTheme theme;
    [ProfileProperty]
    private static LoginMethod loginMethod = LoginMethod.PasswordLogin;
    [ProfileProperty]
    private static string ignoreVersion  = string.Empty;
}
