namespace XCCChatRoom.Controls;

public partial class GroupCardView : ContentView
{
    public static readonly BindableProperty GroupNameProperty = BindableProperty.Create(nameof(GroupName), typeof(string), typeof(GroupCardView), string.Empty);
    public static readonly BindableProperty UserNameInGroupProperty = BindableProperty.Create(nameof(UserNameInGroup), typeof(string), typeof(GroupCardView), string.Empty);
    public string GroupName
    {
        get => (string)GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
    }
    public string UserNameInGroup
    {
        get => (string)GetValue(UserNameInGroupProperty);
        set => SetValue(UserNameInGroupProperty, value);
    }
    public event EventHandler<GroupCardViewClickEventArgs> Click;
    public event EventHandler<GroupCardViewSwipeEventArgs> Swipe;
    private bool isSwiped = false;

    public GroupCardView()
    {
        InitializeComponent();
        this.BindingContext = this;
        this.ScaleTo(1, 200, Easing.CubicOut);
        this.FadeTo(1, 200, Easing.CubicOut);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Click?.Invoke(this, new GroupCardViewClickEventArgs(GroupName, UserNameInGroup, e));
    }


    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (e.StatusType == GestureStatus.Started)
        {
            isSwiped = false;
        }
        if (e.TotalX < -10 && e.StatusType != GestureStatus.Completed && !isSwiped)
        {
            isSwiped = true;
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            Swipe?.Invoke(this, new GroupCardViewSwipeEventArgs(GroupName, e));
        }
    }
}

public class GroupCardViewClickEventArgs : EventArgs
{
    public string GroupName { get; set; }
    public string UserNameInGroup { get; set; }
    public TappedEventArgs TappedEventArgs { get; set; }
    internal GroupCardViewClickEventArgs(string groupName, string userNameInGroup, TappedEventArgs tappedEventArgs)
    {
        GroupName = groupName;
        UserNameInGroup = userNameInGroup;
        TappedEventArgs = tappedEventArgs;
    }
}
public class GroupCardViewSwipeEventArgs : EventArgs
{
    public string GroupName { get; set; }
    public PanUpdatedEventArgs SwipedEventArgs { get; set; }
    internal GroupCardViewSwipeEventArgs(string groupName, PanUpdatedEventArgs swipedEventArgs)
    {
        GroupName = groupName;
        SwipedEventArgs = swipedEventArgs;
    }
}