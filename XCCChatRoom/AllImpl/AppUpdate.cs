using MauiPopup;
using System.Diagnostics;
using XCCChatRoom.Controls;
using XCCChatRoom.Profiles;
using XFEExtension.NetCore.TaskExtension;

namespace XCCChatRoom.AllImpl;

public class AppUpdate
{
    public static bool IsCheckUpdate { get; set; } = false;
    public static bool IsFirstShowException { get; set; } = false;
    public static void StartCheckUpdate(Page currentPage)
    {
#if ANDROID
        new Action(async () =>
        {
            while (!IsCheckUpdate)
            {
                try
                {
                    using var httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.GetAsync("UpdateUrl");

                    if (response.IsSuccessStatusCode)
                    {
                        IsCheckUpdate = true;
                        string content = await response.Content.ReadAsStringAsync();  
                        Trace.WriteLine(content);
                        string[] versionInfo = content.Split('|');
                        if (versionInfo.Length >= 4 && versionInfo[0] != VersionTracking.CurrentVersion && versionInfo[0] != SystemProfile.IgnoreVersion)
                        {
                            if (versionInfo[1] == "0")
                            {
                                currentPage.Dispatcher.Dispatch(async () =>
                                {
                                    var popup = new UpdatePopup("更新啦！", versionInfo[0], $"更新内容：\n{versionInfo[2]}", true, versionInfo[3]);
                                    await PopupAction.DisplayPopup(popup);
                                });
                            }
                            else if (versionInfo[1] == "1")
                            {
                                currentPage.Dispatcher.Dispatch(async () =>
                                {
                                    var popup = new UpdatePopup("更新啦！", versionInfo[0], $"更新内容：\n{versionInfo[2]}", false);
                                    await PopupAction.DisplayPopup(popup);
                                    Process.GetCurrentProcess().Kill();
                                });
                            }
                        }
                    }
                    else
                    {
                        if (IsFirstShowException)
                            await currentPage.DisplayAlert("错误", $"检查更新时发生错误：\n错误码：{response.StatusCode}\n请检查网络设置", "确定");
                        IsCheckUpdate = false;
                    }
                }
                catch (Exception ex)
                {
                    if (IsFirstShowException)
                        await currentPage.DisplayAlert("错误", $"检查更新时发生错误：\n{ex.Message}\n请检查网络设置", "确定");
                    IsCheckUpdate = false;
                }
                Thread.Sleep(5000);
            }
        }).StartNewTask();
#endif
    }
}
