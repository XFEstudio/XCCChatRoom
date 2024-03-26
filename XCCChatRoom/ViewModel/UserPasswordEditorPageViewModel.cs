using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Model;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom.ViewModel;

internal partial class UserPasswordEditorPageViewModel(UserPasswordEditorPage viewPage) : ObservableObject
{
    [ObservableProperty]
    private string originPassword;
    [ObservableProperty]
    private string newPassword;
    [ObservableProperty]
    private string confirmPassword;
    public UserPasswordEditorPage ViewPage { get; init; } = viewPage;
    private bool originPasswordValid = false;
    private bool newPasswordValid = false;
    private bool confirmPasswordValid = false; /*确认按钮允许点击的条件*/

    internal void OriginPassword_Unfocused(object sender, FocusEventArgs e)
    {
        if (UserInfoProfile.CurrentUser.Apassword == OriginPassword)
            originPasswordValid = true;
        else
            originPasswordValid = false;
    }

    internal void NewPassword_Unfocused(object sender, FocusEventArgs e)
    {
        if (NewPassword.IsValidPassword())
            newPasswordValid = true;
        else
            newPasswordValid = false;
    }

    internal void NewPasswordConfirmation_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (newPasswordValid)
        {
            if (NewPassword == ConfirmPassword)
                confirmPasswordValid = true;
            else
                confirmPasswordValid = false;
        }
    }

    internal void ForgotPasswordButton_Click(object sender, TappedEventArgs e) => Shell.Current.GoToAsync(nameof(ForgetPasswordPage));

    [RelayCommand]
    private async Task Confirm()
    {
        if (originPasswordValid && newPasswordValid && confirmPasswordValid)
        {
            UserInfoPage.EditUserProperty(UserPropertyToEdit.Password, NewPassword, ViewPage);
            await Shell.Current?.DisplayAlert("提示", "修改成功", "返回");
            Shell.Current.SendBackButtonPressed();
        }
        else if (!originPasswordValid)
        {
            await Shell.Current?.DisplayAlert("出错了", "旧密码错误", "确定");
        }
        else if (!newPasswordValid)
        {
            await Shell.Current?.DisplayAlert("出错了", "新密码不符合要求", "确定");
        }
        else if (!confirmPasswordValid)
        {
            await Shell.Current?.DisplayAlert("出错了", "两次输入新密码不一致", "确定");
        }
    }
}
