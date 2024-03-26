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
        if (!Directory.Exists(AppPath.ChatImageSavePath))
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
    }
}