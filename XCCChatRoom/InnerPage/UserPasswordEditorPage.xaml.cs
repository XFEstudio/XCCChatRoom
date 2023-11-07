using XCCChatRoom.AllImpl;

namespace XCCChatRoom.InnerPage;

public partial class UserPasswordEditorPage : ContentPage
{
    private bool flag1 = false;
    private bool flag2 = false;
    private bool flag3 = false; /*确认按钮允许点击的条件*/
	public UserPasswordEditorPage()
	{
		InitializeComponent();
	}

    private async void OldPassword_Unfocused(object sender, FocusEventArgs e)
    {
        if (UserInfo.CurrentUser.Apassword == OldPassword.Text) { flag1 = true; }
        else { await DisplayAlert("出错了", "密码错误", "确定"); }
    }


    private async void NewPassword_Unfocused(object sender, FocusEventArgs e)
    {
        if (NewPassword.Text.PasswordEditor()) { flag2 = true; }
        else { await DisplayAlert("出错了", "密码不符合要求", "确定"); }
    }

    private void NewPasswordConfirmation_TextChanged(object sender, EventArgs e)
    {
        if(NewPassword.Text == NewPasswordConfirmation.Text) { flag3 = true; }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if(flag1 && flag2 && flag3)
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.Password,NewPassword.Text, this);
            await DisplayAlert("提示", "修改成功", "返回");
        }
        else
        {
            await DisplayAlert("提示", "请按照要求输入内容","确定");
        }
    }
}