using MauiPopup;
using MauiPopup.Views;
using XCCChatRoom.AllImpl;

namespace XCCChatRoom.Controls;

public partial class ImagePopup : BasePopupPage
{
    public byte[] ImageBuffer { get; set; }
    public string ImageId { get; set; }
    public ImagePopup(byte[] imageBuffer, string imageId)
    {
        ImageBuffer = imageBuffer;
        ImageId = imageId;
        InitializeComponent();
        imageView.Source = ImageSource.FromStream(() => new MemoryStream(imageBuffer));
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        var result = await Permissions.RequestAsync<Permissions.StorageWrite>();
        switch (result)
        {
            case PermissionStatus.Unknown:
                await DisplayAlert("储存失败", "未知请求状态", "啊？");
                break;
            case PermissionStatus.Denied:
                await DisplayAlert("储存失败", "请求被拒绝", "确认");
                break;
            case PermissionStatus.Disabled:
                await DisplayAlert("储存失败", "储存不了了捏，得去手动赋予权限", "OK");
                break;
            case PermissionStatus.Granted:
                if (Directory.Exists(AppPath.ChatImageSavePath))
                    Directory.CreateDirectory(AppPath.ChatImageSavePath);
                var savePath = $"{AppPath.ChatImageSavePath}/{ImageId}.png";
                try
                {
                    await File.WriteAllBytesAsync(savePath, ImageBuffer);
                    await PopupAction.DisplayPopup(new TipPopup("保存成功", $"图片已保存到：{savePath}", 1.5));
                }
                catch (Exception ex)
                {
                    await PopupAction.DisplayPopup(new ErrorPopup("保存失败", $"出现错误：{ex.Message}"));
                }
                break;
            case PermissionStatus.Restricted:
                await DisplayAlert("储存失败", "被Restricted了", "我测");
                break;
            case PermissionStatus.Limited:
                await DisplayAlert("储存失败", "请求被限制了", "啊？");
                break;
            default:
                break;
        }
    }
}