using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom.ViewModel;

internal partial class AICenterPageViewModel(AICenterPage viewPage) : ObservableObject
{
    public AICenterPage ViewPage { get; init; } = viewPage;

    [RelayCommand]
    static async Task ChatToNormalGPTClick()
    {
        if (UserInfoProfile.LoginSuccessful)
        {
            await Shell.Current?.GoToAsync(nameof(GPTAIChatPage));
        }
        else
        {
            if (await Shell.Current?.DisplayAlert("尚未登录", "您还没有登录，无法使用该功能", "前往登录", "取消"))
            {
                await Shell.Current?.GoToAsync(nameof(UserLoginPage));
            }
        }
    }

    [RelayCommand]
    static async Task ChatToAdvanceGPTClick()
    {
        await Shell.Current?.DisplayAlert("功能尚未开发", "目前该功能还未开发完成，敬请期待", "急急急");
        //Shell.Current.GoToAsync(nameof(GPTAIChatPage));
    }

    [RelayCommand]
    static async Task AIDrawPictureClick()
    {
        await Shell.Current?.DisplayAlert("功能尚未开发", "目前该功能还未开发完成，敬请期待", "这也没有？");
        //Shell.Current.GoToAsync(nameof(AIDrawPage));
    }

    [RelayCommand]
    static async Task TapGestureRecognizerTap()
    {
        await Shell.Current?.DisplayAlert("关于AI", "目前正在持续开发中，早期测试版很多功能有待完善，给您带来的不便还请谅解", "了解");
    }
}
