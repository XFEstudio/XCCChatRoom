using XCCChatRoom.ViewModel;

namespace XCCChatRoom.ViewPage;

public partial class GroupContactPage : ContentPage
{
    internal GroupContactPageViewModel ViewModel { get; init; }
    public static GroupContactPage Current { get; set; }

    public GroupContactPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
        Current = this;
    }
}