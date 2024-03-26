using XCCChatRoom.ViewModel;
using XFEExtension.NetCore.ArrayExtension;

namespace XCCChatRoom.ViewPage;

public partial class PostEditPage : ContentPage
{
    internal PostEditPageViewModel ViewModel { get; init; }
    public PostEditPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (titleEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
        (contentEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    protected override bool OnBackButtonPressed()
    {
        if (!ViewModel.backTrigger && !ViewModel.secTrigger)
        {
            this.Dispatcher.Dispatch(async () =>
            {
                if (await Shell.Current?.DisplayAlert("确定退出吗？", "未保存的内容将会丢失，请谨慎选择", "退出", "取消"))
                {
                    ViewModel.secTrigger = true;
                    Shell.Current.SendBackButtonPressed();
                }
                else
                {
                    ViewModel.secTrigger = false;
                }
            });
            return true;
        }
        else
        {
            return base.OnBackButtonPressed();
        }
    }

    private void ContentEditor_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.ContentEditor_TextChanged(sender, e);
}