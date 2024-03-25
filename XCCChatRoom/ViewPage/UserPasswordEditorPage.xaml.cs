using XCCChatRoom.AllImpl;
using XCCChatRoom.Model;

namespace XCCChatRoom.ViewPage;

public partial class UserPasswordEditorPage : ContentPage
{
    private bool flag1 = false;
    private bool flag2 = false;
    private bool flag3 = false; /*确认按钮允许点击的条件*/
	public UserPasswordEditorPage()
	{
		InitializeComponent();
	}


    private void OldPassword_Unfocused(object sender, FocusEventArgs e)
    {
        if (UserInfoPage.CurrentUser.Apassword == OldPasswordEntry.Text) { flag1 = true; }
        else { flag1 = false; }
    }


    private void NewPassword_Unfocused(object sender, FocusEventArgs e)
    {
        if (NewPasswordEntry.Text.IsValidPassword()) { flag2 = true; }
        else { flag2 = false; }
    }

    private void NewPasswordConfirmation_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (flag2)
        {
            if (NewPasswordEntry.Text == NewPasswordConfirmationEntry.Text) { flag3 = true; }
            else { flag3 = false; }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if(flag1 && flag2 && flag3)
        {
            UserInfoPage.EditUserProperty(UserPropertyToEdit.Password, NewPasswordEntry.Text, this);
            await Shell.Current?.DisplayAlert("提示", "修改成功", "返回");
            Shell.Current.SendBackButtonPressed();
        }
        else if(!flag1)
        {
            await Shell.Current?.DisplayAlert("出错了", "旧密码错误", "确定");
        }
        else if(!flag2)
        {
            await Shell.Current?.DisplayAlert("出错了", "新密码不符合要求", "确定");
        }
        else if(!flag3)
        {
            await Shell.Current?.DisplayAlert("出错了", "两次输入新密码不一致", "确定");
        }
    }

    private void ForgotPasswordButton_Click(object sender, TappedEventArgs e)
    {
        Shell.Current.GoToAsync(nameof(ForgetPasswordPage));
    }
}