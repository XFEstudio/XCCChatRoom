using XFEExtension.NetCore.ProfileExtension;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.Profiles;

public static partial class UserInfoProfile
{
    [ProfileProperty]
    private static bool loginSuccessful;
    [ProfileProperty]
    private static string uUID;
    [ProfileProperty]
    private static string name;
    [ProfileProperty]
    private static string email;
    [ProfileProperty]
    private static string password;
    [ProfileProperty]
    private static string phone;
    [ProfileProperty]
    [ProfilePropertyAddGet(@"currentUser.Agroups ??= string.Empty")]
    [ProfilePropertyAddGet(@"currentUser.LikedPostID ??= string.Empty")]
    [ProfilePropertyAddGet(@"currentUser.LikedCommentID ??= string.Empty")]
    [ProfilePropertyAddGet(@"currentUser.LatestMessage ??= string.Empty")]
    [ProfilePropertyAddGet(@"currentUser.StarredPostID ??= string.Empty")]
    [ProfilePropertyAddGet("return currentUser")]
    private static XFEChatRoom_UserInfoForm currentUser;
}
