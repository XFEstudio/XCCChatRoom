using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom.ViewModel;

internal partial class IntroductionPageViewModel : ObservableObject
{
    [ObservableProperty]
    private int pageCount;
    public IntroductionPage ViewPage { get; init; }

    public IntroductionPageViewModel(IntroductionPage viewPage)
    {
        ViewPage = viewPage;
        ViewPage.Loaded += ViewPageLoaded;
        PageCount = 1;
    }

    private async void ViewPageLoaded(object sender, EventArgs e)
    {
        _ = ViewPage.titleLabel.FadeTo(1, 800, Easing.CubicOut);
        _ = ViewPage.titleLabel.TranslateTo(0, 0, 800, Easing.CubicOut);
        await Task.Delay(100);
        _ = ViewPage.showImage.FadeTo(1, 800, Easing.CubicOut);
        _ = ViewPage.showImage.TranslateTo(0, 0, 800, Easing.CubicOut);
        await Task.Delay(100);
        _ = ViewPage.contentLabel.FadeTo(0.9, 800, Easing.CubicOut);
        _ = ViewPage.contentLabel.TranslateTo(0, 0, 800, Easing.CubicOut);
        await Task.Delay(100);
        _ = ViewPage.indexLabel.FadeTo(0.5, 800, Easing.CubicOut);
        _ = ViewPage.indexLabel.TranslateTo(0, 0, 800, Easing.CubicOut);
        await Task.Delay(100);
        _ = ViewPage.nextButton.FadeTo(1, 800, Easing.CubicOut);
        _ = ViewPage.nextButton.TranslateTo(0, 0, 800, Easing.CubicOut);
        await Task.Delay(100);
        _ = ViewPage.skipButton.FadeTo(0.7, 800, Easing.CubicOut);
        await ViewPage.skipButton.TranslateTo(0, 0, 800, Easing.CubicOut);
    }

    private async void OutAndInAnimation(string titleText, string contentText, string imageResource)
    {
        ViewPage.nextButton.IsEnabled = false;
        _ = ViewPage.titleLabel.TranslateTo(0, -150, 800, Easing.SpringOut);
        _ = ViewPage.showImage.TranslateTo(-150, 0, 800, Easing.SpringOut);
        _ = ViewPage.contentLabel.TranslateTo(-150, 0, 800, Easing.SpringOut);
        _ = ViewPage.indexLabel.TranslateTo(150, 0, 800, Easing.SpringOut);
        _ = ViewPage.titleLabel.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.showImage.FadeTo(0, 800, Easing.SpringOut);
        _ = ViewPage.contentLabel.FadeTo(0, 800, Easing.SpringOut);
        await ViewPage.indexLabel.FadeTo(0, 800, Easing.SpringOut);
        ViewPage.titleLabel.Text = titleText;
        ViewPage.contentLabel.Text = contentText;
        ViewPage.showImage.Source = imageResource;
        ViewPage.titleLabel.TranslationX = 0;
        ViewPage.titleLabel.TranslationY = 0;
        ViewPage.showImage.TranslationX = 0;
        ViewPage.showImage.TranslationY = 0;
        ViewPage.contentLabel.TranslationX = 0;
        ViewPage.contentLabel.TranslationY = 0;
        ViewPage.indexLabel.TranslationX = 0;
        ViewPage.indexLabel.TranslationY = 0;
        ViewPage.nextButton.IsEnabled = true;
        _ = ViewPage.titleLabel.FadeTo(1, 800, Easing.SpringOut);
        _ = ViewPage.showImage.FadeTo(1, 800, Easing.SpringOut);
        _ = ViewPage.contentLabel.FadeTo(0.9, 800, Easing.SpringOut);
        await ViewPage.indexLabel.FadeTo(0.5, 800, Easing.SpringOut);
    }

    internal async void SkipPage(object sender, TappedEventArgs e)
    {
        PageCount = 7;
        await NextPage();
    }

    [RelayCommand]
    private async Task NextPage()
    {
        PageCount++;
        switch (PageCount)
        {
            case 2:
                OutAndInAnimation("畅所欲言", "在这里，你可以说一切你想说的\n此外，我们还有社区功能，尽情发挥你的灵感吧！", "being_creative");
                break;
            case 3:
                OutAndInAnimation("AI时代", "在这个AI的时代，我们的软件怎么能少得了它呢？\n我们的软件内有AI聊天和AI创作等，可以解决你方方面面的问题。千万不要小瞧了AI", "artificial_intelligence");
                break;
            case 4:
                OutAndInAnimation("聊天表情", "如果说一个聊天软件枯燥在哪，那我想一定就是没有表情包了吧，因而我们内置了一套简约风的表情包，不臃肿，不简陋", "emoji_discuss");
                break;
            case 5:
                OutAndInAnimation("我们的团队", "我们有一个因为兴趣爱好吸引到一起的开发团队，人虽然不多，但是我们都很勤奋", "about_our_team");
                break;
            case 6:
                OutAndInAnimation("关于测试版", "我们的软件目前还是处于内测阶段，如果您发现了任何bug都可以通知我们", "beta_testing");
                break;
            case 7:
                OutAndInAnimation("创作灵感", "您的灵感才是我们更新的来源！虽然我们有很多的灵感，但他们迟早有用完的一天，是您这样的人才丰富了我们的社区", "get_inspired");
                break;
            case 8:
                ViewPage.nextButton.Text = "完成";
                ViewPage.nextButton.Margin = new Thickness(20, 10, 20, 50);
                ViewPage.skipButton.IsVisible = false;
                OutAndInAnimation("完成", "现在一切都准备就绪啦，让我们开始吧！", "success");
                break;
            case 9:
                ViewPage.nextButton.IsEnabled = false;
                await ViewPage.FadeTo(0, 1500);
                var mainShell = new AppShell
                {
                    Opacity = 0
                };
                await mainShell.FadeTo(1, 300, Easing.CubicOut);
                Application.Current.MainPage = mainShell;
                break;
            default:
                throw new Exception("页面超出范围");
        }
    }
}
