﻿namespace XCCChatRoom.AllImpl
{
    public static class AppPath
    {
        public static string ProfilesPath { get; } = $"{FileSystem.Current.CacheDirectory}/Profiles";
        public static string SystemProfilePath { get; } = $"{FileSystem.Current.CacheDirectory}/SystemProfile.xfe";
        public static string GPTAIDialogsPath { get; } = $"{FileSystem.Current.CacheDirectory}/GPTAIDialogs.xfe";
        public static string ChatDialogHistoryPath { get; } = $"{FileSystem.Current.CacheDirectory}/ChatDialogHistory";
        public static string CheckInitializePath { get; } = $"{FileSystem.Current.AppDataDirectory}/Initialize.xfe";
        public static string AndroidExternalPath { get; } = "/storage/emulated/0";
        public static string AppExternalPath { get; } = $"{AndroidExternalPath}/Android/data/com.xfegzs.xccchatroom";
        public static string ChatImageSavePath { get; } = $"{AppExternalPath}/ChatImage";
    }
}
