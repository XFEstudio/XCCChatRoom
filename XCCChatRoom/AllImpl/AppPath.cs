namespace XCCChatRoom.AllImpl;

public static class AppPath
{
    public static string ProfilesPath { get; } = $"{FileSystem.Current.AppDataDirectory}/Profiles";
    public static string DownloadRootPath { get; } = "/storage/emulated/0/Download";
    public static string PicturesRootPath { get; } = "/storage/emulated/0/Pictures";
    public static string AppDownloadFileSavePath { get; } = $"{DownloadRootPath}/XCCChatRoom";
    public static string AppChatImageSavePath { get; } = $"{PicturesRootPath}/XCCChatImage";
    public static string AppUpgradePath { get; } = $"{AppDownloadFileSavePath}/UpgradePackages";
    public static string GPTAIDialogsPath { get; } = $"{FileSystem.Current.CacheDirectory}/GPTAIDialogs.xfe";
    public static string ChatDialogHistoryPath { get; } = $"{FileSystem.Current.CacheDirectory}/ChatDialogHistory";
    public static string AndroidExternalPath { get; } = "/storage/emulated/0";
    public static string AppExternalPath { get; } = $"{AndroidExternalPath}/Android/data/com.xfegzs.xccchatroom";
}
