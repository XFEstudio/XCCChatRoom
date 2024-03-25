using CommunityToolkit.Mvvm.ComponentModel;
using XCCChatRoom.AllImpl;
using XCCChatRoom.ViewPage;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.ViewModel;

public partial class UserInfoPageViewModel : ObservableObject
{
    private static XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
    [ObservableProperty]
    private string userName;
    [ObservableProperty]
    private string userNameFirstLatter;
    [ObservableProperty]
    private string uUID;

    public UserInfoPage ViewPage { get; set; }

    public UserInfoPageViewModel(UserInfoPage viewPage)
    {
        ViewPage = viewPage;
        if (UserInfoPage.IsLoginSuccessful)
        {
            UserName = UserInfoPage.StaticUserName;
            UUID = UserInfoPage.StaticUUID;
            SwitchToLoginStyle();
        }
        else
        {
            UserNameFirstLatter = "?";
            UserName = "未登录";
            UUID = "暂无UID";
            UserInfoPage.StaticUserName = string.Empty;
            UserInfoPage.StaticUUID = string.Empty;
            UserInfoPage.StaticPassword = string.Empty;
            UserInfoPage.StaticPhoneNum = string.Empty;
        }
    }
}
