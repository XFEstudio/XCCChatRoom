using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPopup;
using System.Diagnostics;
using XCCChatRoom.AllImpl;
using XFEExtension.NetCore.FileExtension;
using XFEExtension.NetCore.WebExtension;

namespace XCCChatRoom.Controls.ViewModel;

internal partial class UpdatePopupViewModel : ObservableObject
{
    [ObservableProperty]
    private string titleText;
    [ObservableProperty]
    private string versionText;
    [ObservableProperty]
    private string contentText;
    [ObservableProperty]
    private string downloadProcessText = "准备下载中...";
    [ObservableProperty]
    private double downloadProgress;
    public UpdatePopup ViewPage { get; init; }
    private XFEDownloader xFEDownloader;
    private readonly string versionNumber;
    private readonly string saveFilePath;

    public UpdatePopupViewModel(UpdatePopup viewPage, string updateTitle, string updateVersion, string updateContent, bool cancelable, string lastForceDownloadVersion)
    {
        ViewPage = viewPage;
        ThemeImageGetter.Add(ViewPage.pauseAndContinueButton);
        if (cancelable)
            VersionText = $"版本：{updateVersion}";
        else
            VersionText = $"版本：{updateVersion}（强制更新）";
        if (Version.TryParse(lastForceDownloadVersion, out var version))
        {
            if (Version.Parse(VersionTracking.CurrentVersion) <= version)
            {
                cancelable = false;
                VersionText = $"版本：{updateVersion}（最低支持版本：{version}）";
            }
        }
        if (!cancelable)
        {
            ViewPage.cancelButton.IsVisible = false;
            ViewPage.skipButton.IsVisible = false;
            ViewPage.IsCloseOnBackgroundClick = false;
        }
        TitleText = updateTitle;
        ContentText = updateContent;
        versionNumber = updateVersion;
        saveFilePath = $"{AppPath.AppUpgradePath}/com.xfegzs.xccchatroom.{versionNumber}.apk";
    }

    private async void XFEDownloader_BufferDownloaded(XFEDownloader sender, FileDownloadedEventArgs e)
    {
        try
        {
            DownloadProgress = e.DownloadedBufferSize / (double)e.TotalBufferSize;
            DownloadProcessText = $"{e.DownloadedBufferSize.FileSize()}/{((long)e.TotalBufferSize).FileSize()}";
            if (e.Downloaded)
            {
                ShowInstallButton();
                await Install();
            }
        }
        catch { }
    }

    private void ShowInstallButton()
    {
        ViewPage.Dispatcher.Dispatch(() =>
        {
            ViewPage.installButton.IsVisible = true;
            ViewPage.downloadGrid.IsVisible = false;
        });
    }

    [RelayCommand]
    private async Task Upgrade()
    {
        try
        {
            ViewPage.updateButton.IsVisible = false;
            ViewPage.downloadGrid.IsVisible = true;
            if (!Directory.Exists(AppPath.AppUpgradePath))
                Directory.CreateDirectory(AppPath.AppUpgradePath);
            foreach (var file in Directory.GetFiles(AppPath.AppUpgradePath))
                if (file != saveFilePath)
                    File.Delete(file);
            xFEDownloader = new()
            {
                DownloadUrl = "AppUrl",
                SavePath = saveFilePath
            };
            xFEDownloader.BufferDownloaded += XFEDownloader_BufferDownloaded;
            if (File.Exists(saveFilePath))
                await xFEDownloader.Continue();
            else
                await xFEDownloader.Download();
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
            await Shell.Current!.DisplayAlert("更新发生错误", $"错误内容：{ex.Message}", "确认");
            await PopupAction.ClosePopup();
        }
    }

    [RelayCommand]
    private static async Task Cancel() => await PopupAction.ClosePopup();

    [RelayCommand]
    private void PauseOrContinue()
    {
        if (xFEDownloader.IsPaused)
        {
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.pauseAndContinueButton.ClassId = ViewPage.pauseAndContinueButton.ClassId.Replace("play", "pause");
                ViewPage.pauseAndContinueButton.Source = ViewPage.pauseAndContinueButton.ClassId;
            });
            _ = Task.Run(xFEDownloader.Continue);
        }
        else
        {
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.pauseAndContinueButton.ClassId = ViewPage.pauseAndContinueButton.ClassId.Replace("pause", "play");
                ViewPage.pauseAndContinueButton.Source = ViewPage.pauseAndContinueButton.ClassId;
            });
            xFEDownloader.Pause();
        }
    }

    [RelayCommand]
    private async Task Install()
    {
        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(saveFilePath)
        });
    }
}
