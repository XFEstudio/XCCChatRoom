using XFE各类拓展.StringExtension;
using XFE各类拓展.TaskExtension;

namespace XCCChatRoom.AllImpl
{
    public class AppUpdate
    {
        public static bool IsCheckUpdate { get; set; } = false;
        public static bool IsFirstShowException { get; set; } = false;
        public static void StartCheckUpdate(Page CurrentPage)
        {
#if ANDROID
            new Action(async () =>
            {
                while (!IsCheckUpdate)
                {
                    try
                    {
                        using HttpClient httpClient = new HttpClient();
                        HttpResponseMessage response = await httpClient.GetAsync("https://www.xfegzs.com/XFElts/XCCVersion.xfe");

                        if (response.IsSuccessStatusCode)
                        {
                            IsCheckUpdate = true;
                            string content = await response.Content.ReadAsStringAsync();
                            content.CW();
                            string[] versionInfo = content.Split('|');
                            if (versionInfo.Length == 3 && versionInfo[0] != VersionTracking.CurrentVersion && versionInfo[0] != AppSystemProfile.IgnoreVersion)
                            {
                                //非强制更新
                                if (versionInfo[1] == "0")
                                {
                                    CurrentPage.Dispatcher.Dispatch(async () =>
                                    {
                                        if (await CurrentPage.DisplayAlert("更新", $"检测到新版本V{versionInfo[0]}，是否更新？\n更新内容：\n{versionInfo[2]}", "更新", "取消"))
                                        {
                                            await Launcher.OpenAsync(new Uri("https://www.xfegzs.com/com.xfegzs.xccchatroom.apk"));
                                        }
                                    });
                                }
                                //强制更新
                                else if (versionInfo[1] == "1")
                                {
                                    CurrentPage.Dispatcher.Dispatch(async () =>
                                    {
                                        await CurrentPage.DisplayAlert("更新", $"检测到新版本V{versionInfo[0]}，请更新（该版本为强制更新）！\n更新内容：\n{versionInfo[2]}", "确定");
                                        await Launcher.OpenAsync(new Uri("https://www.xfegzs.com/com.xfegzs.xccchatroom.apk"));
#if ANDROID
                                        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
                                    });
                                }
                            }
                        }
                        else
                        {
                            if (IsFirstShowException)
                                await CurrentPage.DisplayAlert("错误", $"检查更新时发生错误：\n错误码：{response.StatusCode}\n请检查网络设置", "确定");
                            IsCheckUpdate = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (IsFirstShowException)
                            await CurrentPage.DisplayAlert("错误", $"检查更新时发生错误：\n{ex.Message}\n请检查网络设置", "确定");
                        IsCheckUpdate = false;
                    }
                    Thread.Sleep(5000);
                }
            }).StartNewTask();
#endif
        }
    }
}
