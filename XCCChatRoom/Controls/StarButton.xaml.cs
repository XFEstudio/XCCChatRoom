namespace XCCChatRoom.Controls;

public partial class StarButton : ContentView
{
    private bool isStar;

    public bool IsStar
    {
        get { return isStar; }
        set
        {
            isStar = value;
            if (isStar)
                starButton.Source = "star";
            else
                starButton.Source = "unstar";
        }
    }

    public event EventHandler Clicked;
    public StarButton()
    {
        InitializeComponent();
    }

    private async void starButton_Clicked(object sender, EventArgs e)
    {
        starButton.IsEnabled = false;
        if (IsStar)
        {
            await starButton.ScaleTo(0, 200, Easing.CubicOut);
            IsStar = false;
            Clicked?.Invoke(this, e);
            await starButton.ScaleTo(1, 200, Easing.CubicOut);
            starButton.IsEnabled = true;
        }
        else
        {
            await starButton.ScaleTo(0, 200, Easing.CubicOut);
            IsStar = true;
            Clicked?.Invoke(this, e);
            circleImage.Opacity = 1;
            circleImage.Scale = 0;
            circleImage.IsVisible = true;
            var task = starButton.ScaleTo(1, 200, Easing.CubicOut);
            var task2 = circleImage.ScaleTo(2, 300, Easing.CubicOut);
            await circleImage.FadeTo(0, 300, Easing.CubicOut);
            circleImage.IsVisible = false;
            starButton.IsEnabled = true;
        }
    }
}