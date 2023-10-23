namespace XCCChatRoom.AllImpl
{
    public static class AppPath
    {
        public static string UserInfoPath { get; private set; } = $"{FileSystem.Current.CacheDirectory}/UserInfo.xfe";
        public static string SystemProfilePath { get; private set; } = $"{FileSystem.Current.CacheDirectory}/SystemProfile.xfe";
        public static string GPTAIDialogsPath { get; private set; } = $"{FileSystem.Current.CacheDirectory}/GPTAIDialogs.xfe";
        public static string CheckInitializePath { get; private set; } = $"{FileSystem.Current.AppDataDirectory}/Initialize.xfe";
    }
}
