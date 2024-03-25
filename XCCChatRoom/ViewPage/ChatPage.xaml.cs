using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;
[QueryProperty(nameof(GroupName), nameof(GroupName))]
[QueryProperty(nameof(CurrentName), nameof(CurrentName))]
public partial class ChatPage : ContentPage
{
    #region 公共属性
    public string GroupName
    {
        get => ViewModel.GroupName;
        set
        {
            ViewModel.GroupName = value;
            ViewModel.DisplayGroupName = $"群组|{value}";
        }
    }
    public static ChatPage Current { get; private set; }
    public string CurrentName
    {
        get => ViewModel.CurrentName;
        set
        {
            ViewModel.CurrentName = value;
        }
    }
    public ChatPageViewModel ViewModel { get; init; }
    #endregion
    public ChatPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
        Current = this;
    }

    protected override bool OnBackButtonPressed()
    {
        if (ViewModel.PhoneCallEnabled)
        {
            phoneCallItem.IconImageSource = "phone";
#if ANDROID
            ViewModel.StopRecording();
            ViewModel.StopPlayback();
#endif
            ViewModel.PhoneCallEnabled = false;
        }
        ViewModel.StopGroupComm();
        return base.OnBackButtonPressed();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (inputEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    private void InputEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.CurrentEditorText))
        {
            sendButton.BackgroundColor = Color.FromArgb("#A491E8");
            sendButton.IsEnabled = false;
        }
        else
        {
            sendButton.BackgroundColor = Color.FromArgb("#512BD4");
            sendButton.IsEnabled = true;
        }
    }

    private void ChatScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        //TODO:
    }

    private void UnFocusAllButtonInToolBar()
    {
        foreach (var btn in toolBarStackLayout.Children.Cast<ImageButton>().Where(btn => !btn.ClassId.Contains("un")))
        {
            btn.Source = "un" + btn.ClassId;
            btn.ClassId = "un" + btn.ClassId;
        }

        this.Dispatcher.Dispatch(() =>
        {
            toolViewStackLayout.Children.Clear();
            toolViewScrollView.IsVisible = false;
        });
    }

    private void InputEditor_Focused(object sender, FocusEventArgs e)
    {
        toolViewScrollView.FadeTo(0, 200, Easing.SpringOut);
        toolViewScrollView.IsVisible = false;
        toolViewStackLayout.Children.Clear();
        UnFocusAllButtonInToolBar();
    }
}