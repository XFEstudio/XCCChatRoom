namespace XCCChatRoom.Controls;

public partial class LikeButton : ContentView
{
    private bool isLike;

    public bool IsLike
    {
        get { return isLike; }
        set
        {
            isLike = value;
            if (isLike)
                likeButton.Source = "like";
            else
                likeButton.Source = "unlike";
        }
    }
    private int targetSize;
    public int TargetSize
    {
        get { return targetSize; }
        set
        {
            targetSize = value;
            likeButton.WidthRequest = targetSize;
            likeButton.HeightRequest = targetSize;
            circleImage.WidthRequest = targetSize;
            circleImage.HeightRequest = targetSize;
        }
    }
    public event EventHandler Clicked;
    public LikeButton()
    {
        InitializeComponent();
    }

    private async void likeButton_Clicked(object sender, EventArgs e)
    {
        likeButton.IsEnabled = false;
        if (IsLike)
        {
            await likeButton.ScaleTo(0, 200, Easing.CubicOut);
            IsLike = false;
            Clicked?.Invoke(this, e);
            await likeButton.ScaleTo(1, 200, Easing.CubicOut);
            likeButton.IsEnabled = true;
        }
        else
        {
            await likeButton.ScaleTo(0, 200, Easing.CubicOut);
            IsLike = true;
            Clicked?.Invoke(this, e);
            circleImage.Opacity = 1;
            circleImage.Scale = 0;
            circleImage.IsVisible = true;
            var task = likeButton.ScaleTo(1, 200, Easing.CubicOut);
            var task2 = circleImage.ScaleTo(3, 300, Easing.CubicOut);
            await circleImage.FadeTo(0, 300, Easing.CubicOut);
            circleImage.IsVisible = false;
            likeButton.IsEnabled = true;
        }
    }
}