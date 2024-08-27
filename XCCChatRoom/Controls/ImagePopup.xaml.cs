using CommunityToolkit.Maui.Alerts;
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
        if (!Directory.Exists(AppPath.AppChatImageSavePath))
            Directory.CreateDirectory(AppPath.AppChatImageSavePath);
        var savePath = $"{AppPath.AppChatImageSavePath}/{ImageId}.png";
        try
        {
            await File.WriteAllBytesAsync(savePath, ImageBuffer);
            await Toast.Make($"图片已保存到：{savePath}").Show();
        }
        catch (Exception ex)
        {
            await Toast.Make($"保存失败：{ex.Message}").Show();
        }
    }
}