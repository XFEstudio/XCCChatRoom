using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPopup;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Model;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.StringExtension;

namespace XCCChatRoom.ViewModel;

internal partial class UserPropertyEditPageViewModel(UserPropertyEditPage viewPage) : ObservableObject
{
    [ObservableProperty]
    private string phoneNum = UserInfoProfile.CurrentUser.Atel;
    [ObservableProperty]
    private string password = "**********";
    [ObservableProperty]
    private string name = UserInfoProfile.CurrentUser.Aname;
    [ObservableProperty]
    private string mail = UserInfoProfile.CurrentUser.Amail;
    public UserPropertyEditPage ViewPage { get; init; } = viewPage;
    private bool modifyAuthentication = false;

    private async void ModifyAuthentication()
    {
        var authenticationPassed = true;
        var randomCode = IDGenerator.SummonRandomID(6);
        if (!modifyAuthentication)
        {
            bool authenticated;
            if (await Shell.Current?.DisplayAlert("当前环境不安全", "请进行身份验证", "确定", "取消"))
            {
                authenticated = true;
                if (authenticationPassed)
                {
                    /*await TencentSms.SendVerifyCode("1922760", "+86" + UserInfo.CurrentUser.Atel, new string[] { randomCode });*/
                }
            }
            else
            {
                authenticated = false;
            }
            if (authenticated)
            {
                string captcha = /*await DisplayPromptAsync("手机号验证", "请输入验证码", "确定", "取消");*/randomCode;
                if (captcha == randomCode)
                {
                    modifyAuthentication = true;
                }
            }
        }
    }


    [RelayCommand]
    private async Task EditMail()
    {
        if (modifyAuthentication)
        {
            /*await Shell.Current.GoToAsync(nameof(UserMailEditorPage));*/
            await Shell.Current?.DisplayAlert("1", "写完请将改行替换为注释内容", "3");
        }
        else
        {
            ModifyAuthentication();
            return;
        }
        var newUserProperty = await Shell.Current.DisplayPromptAsync("修改", "请输入您要修改的邮箱", "确定", "取消");
        bool flag = newUserProperty.IsValidEmail();
        if (flag)
        {
            UserInfoPage.EditUserProperty(UserPropertyToEdit.Mail, newUserProperty, ViewPage);
            await Shell.Current?.DisplayAlert("修改成功", "内容合法", "明白了");
        }
        else
        {
            await Shell.Current?.DisplayAlert("邮箱无效", "请输入有效的邮箱地址", "明白了");
        }
    }

    [RelayCommand]
    private async Task EditPassword()
    {
        if (modifyAuthentication)
            await Shell.Current.GoToAsync(nameof(UserPasswordEditorPage));
        else
            ModifyAuthentication();
    }

    [RelayCommand]
    private async Task EditName()
    {
        if (Name is not null && Name != string.Empty)
        {
            bool flag = Name.IsValidUserName();
            if (flag)
            {
                UserInfoPage.EditUserProperty(UserPropertyToEdit.UserName, Name, ViewPage);
                await Shell.Current?.DisplayAlert("修改成功", "内容合法", "明白了");
            }
            else
            {
                await Shell.Current?.DisplayAlert("非法昵称", "请输入合法昵称", "明白了");
            }
        }
    }

    [RelayCommand]
    private async Task EditPhoneNum()
    {
        //if (modifyAuthentication)
        await PopupAction.DisplayPopup(new UserTelEditPopup(ViewPage));
        //else
        //ModifyAuthentication();
    }
}
