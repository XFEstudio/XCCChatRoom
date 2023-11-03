namespace XCCChatRoom.InnerPage;

public partial class UserTelEditorPage : ContentView
{
	public UserTelEditorPage()
	{
		InitializeComponent();
		OldTel.Text = UserInfo.CurrentUser.Atel;
	}

    private void NewTel_Unfocused(object sender, FocusEventArgs e)
    {

    }

    private void GetCode_Clicked(object sender, EventArgs e)
    {

    }
}