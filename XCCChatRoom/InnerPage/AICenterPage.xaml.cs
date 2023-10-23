namespace XCCChatRoom.InnerPage;

public partial class AICenterPage : ContentPage
{
    public AICenterPage()
    {
        InitializeComponent();
    }

    private async void ChatToNormalGPTButton_Clicked(object sender, EventArgs e)
    {
        if (UserInfo.IsLoginSuccessful)
        {
            await Shell.Current.GoToAsync(nameof(GPTAIChatPage));
        }
        else
        {
            if (await DisplayAlert("尚未登录", "您还没有登录，无法使用该功能", "前往登录", "取消"))
            {
                await Shell.Current.GoToAsync(nameof(UserLoginPage));
            }
        }
    }

    private async void ChatToHigherGPTButton_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("功能尚未开发", "目前该功能还未开发完成，敬请期待", "急急急");
        //Shell.Current.GoToAsync(nameof(GPTAIChatPage));
    }

    private async void AIDrawPictureButton_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("功能尚未开发", "目前该功能还未开发完成，敬请期待", "这也没有？");
        //Shell.Current.GoToAsync(nameof(AIDrawPage));
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("关于AI", "目前正在持续开发中，早期测试版很多功能有待完善，给您带来的不便还请谅解", "了解");
    }
}