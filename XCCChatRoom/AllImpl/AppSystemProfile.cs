using XFE各类拓展.FileExtension;
using XFE各类拓展.FormatExtension;

namespace XCCChatRoom.AllImpl
{
    public enum LoginMethod
    {
        PasswordLogin,
        VerifyCodeLogin
    }
    public class AppSystemProfile
    {
        public static LoginMethod LoginMethod { get; set; }
        public static void SaveSystemProfile()
        {
            string saveString = new XFEDictionary(new string[]
            {
                nameof(LoginMethod), LoginMethod.ToString()
            }).ToString();
            saveString.WriteIn(AppPath.SystemProfilePath);
        }
        public static void LoadSystemProfile(Page page)
        {
            if (AppPath.SystemProfilePath.ReadOut(out string profileString))
            {
                var profileProperty = new XFEDictionary(profileString);
                foreach (var property in profileProperty)
                {
                    switch (property.Header)
                    {
                        case nameof(LoginMethod):
                            switch (property.Content)
                            {
                                case "PasswordLogin":
                                    LoginMethod = LoginMethod.PasswordLogin;
                                    break;
                                case "VerifyCodeLogin":
                                    LoginMethod = LoginMethod.VerifyCodeLogin;
                                    break;
                                default:
                                    page.DisplayAlert("错误", $"配置文件读取错误，未知的类型：{property.Content}", "我测");
                                    break;
                            }
                            break;
                    }
                }
            }
        }
    }
}
