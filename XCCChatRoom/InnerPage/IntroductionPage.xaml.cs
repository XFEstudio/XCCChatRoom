namespace XCCChatRoom.InnerPage;

public partial class IntroductionPage : ContentPage
{
    private int pageCount;

    public int PageCount
    {
        get { return pageCount; }
        set
        {
            pageCount = value;
        }
    }

    public IntroductionPage()
    {
        InitializeComponent();
        PageCount = 1;
        this.Loaded += async (sender, e) =>
        {
#pragma warning disable CS4014
            titleLabel.FadeTo(1, 800, Easing.CubicOut);
            titleLabel.TranslateTo(0, 0, 800, Easing.CubicOut);
            await Task.Delay(100);
            showImage.FadeTo(1, 800, Easing.CubicOut);
            showImage.TranslateTo(0, 0, 800, Easing.CubicOut);
            await Task.Delay(100);
            contentLabel.FadeTo(0.9, 800, Easing.CubicOut);
            contentLabel.TranslateTo(0, 0, 800, Easing.CubicOut);
            await Task.Delay(100);
            indexLabel.FadeTo(0.5, 800, Easing.CubicOut);
            indexLabel.TranslateTo(0, 0, 800, Easing.CubicOut);
            await Task.Delay(100);
            nextButton.FadeTo(1, 800, Easing.CubicOut);
            nextButton.TranslateTo(0, 0, 800, Easing.CubicOut);
            await Task.Delay(100);
            skipButton.FadeTo(0.7, 800, Easing.CubicOut);
            await skipButton.TranslateTo(0, 0, 800, Easing.CubicOut);
#pragma warning restore
        };
    }

    private async void nextButton_Clicked(object sender, EventArgs e)
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
                nextButton.Text = "完成";
                nextButton.Margin = new Thickness(20, 10, 20, 50);
                skipButton.IsVisible = false;
                OutAndInAnimation("完成", "现在一切都准备就绪啦，让我们开始吧！", "success");
                break;
            case 9:
                nextButton.IsEnabled = false;
                await this.FadeTo(0, 1500);
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

    private void skipButton_Click(object sender, TappedEventArgs e)
    {
        PageCount = 7;
        nextButton_Clicked(null, null);
    }
    private async void OutAndInAnimation(string titleText, string contentText, string imageResource)
    {
#pragma warning disable CS4014
        nextButton.IsEnabled = false;
        titleLabel.TranslateTo(0, -150, 800, Easing.SpringOut);
        showImage.TranslateTo(-150, 0, 800, Easing.SpringOut);
        contentLabel.TranslateTo(-150, 0, 800, Easing.SpringOut);
        indexLabel.TranslateTo(150, 0, 800, Easing.SpringOut);
        titleLabel.FadeTo(0, 800, Easing.SpringOut);
        showImage.FadeTo(0, 800, Easing.SpringOut);
        contentLabel.FadeTo(0, 800, Easing.SpringOut);
        await indexLabel.FadeTo(0, 800, Easing.SpringOut);
        if (pageCount < 9)
            indexLabel.Text = $"{pageCount}/8";
        titleLabel.Text = titleText;
        contentLabel.Text = contentText;
        showImage.Source = imageResource;
        titleLabel.TranslationX = 0;
        titleLabel.TranslationY = 0;
        showImage.TranslationX = 0;
        showImage.TranslationY = 0;
        contentLabel.TranslationX = 0;
        contentLabel.TranslationY = 0;
        indexLabel.TranslationX = 0;
        indexLabel.TranslationY = 0;
        nextButton.IsEnabled = true;
        titleLabel.FadeTo(1, 800, Easing.SpringOut);
        showImage.FadeTo(1, 800, Easing.SpringOut);
        contentLabel.FadeTo(0.9, 800, Easing.SpringOut);
        await indexLabel.FadeTo(0.5, 800, Easing.SpringOut);
#pragma warning restore
    }
}