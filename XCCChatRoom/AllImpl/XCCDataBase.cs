using XFE各类拓展.NetCore.WebExtension;
using XFE各类拓展.NetCore.XEAEncryption;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.AllImpl
{
    public static class XCCDataBase
    {
        private static string dataBasePassword;
        public static string DataBasePassword
        {
            get { return dataBasePassword; }
            set
            {
                dataBasePassword = value.XEADecrypt("早期测试版数据库密码")[..^2];
                Console.WriteLine(dataBasePassword);
            }
        }
        public static XFEDataBase XFEDataBase { get; set; }
        public static void Initialize()
        {
            DataBasePassword = "https://www.xfegzs.com/XFEChatRoom/DBKey.xfe".GetFromURL();
            XFEDataBase = new XFEDataBase("xfeaccount", "XFEaccount", DataBasePassword);
        }
    }
}