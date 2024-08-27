using System.Diagnostics;
using XFEExtension.NetCore.WebExtension;
using XFEExtension.NetCore.XEAEncryption;
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
                dataBasePassword = value.XEADecrypt("Password");
                Trace.WriteLine(dataBasePassword);
            }
        }
        public static XFEDataBase XFEDataBase { get; set; }
        public static void Initialize()
        {
            try
            {
                DataBasePassword = "Password";
                XFEDataBase = new XFEDataBase("DataBase", "DataBase", DataBasePassword);
            }
            catch (Exception ex)
            {
                Shell.Current!.DisplayAlert("错误", ex.Message, "确认");
            }
        }
    }
}