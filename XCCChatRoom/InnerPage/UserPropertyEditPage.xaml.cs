using MauiPopup;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XFE各类拓展.NetCore.StringExtension;

namespace XCCChatRoom.InnerPage;

public partial class UserPropertyEditPage : ContentPage
{
    public string CurrentPhoneNumLabelText
    {
        get
        {
            return CurrentUserTelLabel.Text;
        }
        set
        {
            CurrentUserTelLabel.Dispatcher.Dispatch(() => CurrentUserTelLabel.Text = value);
        }
    }
    public string CurrentPasswordLabelText
    {
        get
        {
            return CurrentPasswordLabel.Text;
        }
        set
        {
            CurrentPasswordLabel.Dispatcher.Dispatch(() => CurrentPasswordLabel.Text = value);
        }
    }
    public string CurrentMailLabelText
    {
        get
        {
            return CurrentUserMailLabel.Text;
        }
        set
        {
            CurrentUserMailLabel.Dispatcher.Dispatch(() => CurrentUserMailLabel.Text = value);
        }
    }
    public UserPropertyEditPage()
    {
        InitializeComponent();
        CurrentUserTelLabel.Text = UserInfo.CurrentUser.Aname = UserInfo.CurrentUser.Aname;
        CurrentPhoneNumLabelText = UserInfo.CurrentUser.Atel;
        CurrentMailLabelText = UserInfo.CurrentUser.Amail;
    }
    private static bool modifyAuthentication = false;
    private async void ModifyAuthentication()
    {
        bool flag1 = false;
        bool flag2 = true;
        var randomCode = IDGenerator.SummonRandomID(6);

        if (!modifyAuthentication)
        {
            bool telFlag = await DisplayAlert("当前环境不安全", "请进行身份验证", "确定", "取消");
            if (telFlag)
            {
                flag1 = true;
                if (flag2)
                {
                    /*await TencentSms.SendVerifyCode("1922760", "+86" + UserInfo.CurrentUser.Atel, new string[] { randomCode });*/
                    flag2 = false;
                }
            }
            else { flag1 = false; }
            if (flag1)
            {
                string captcha = /*await DisplayPromptAsync("手机号验证", "请输入验证码", "确定", "取消");*/randomCode;
                if (captcha == randomCode)
                {
                    modifyAuthentication = true;
                }
            }
        }

    }
    private async void UserNameEditor()
    {
        if (CurrentUserNameEntry.Text is not null && CurrentUserNameEntry.Text != string.Empty)
        {
            bool flag = CurrentUserNameEntry.Text.UserNameEditor();
            if (flag)
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.UserName, CurrentUserNameEntry.Text, this);
                await DisplayAlert("修改成功", "内容合法", "明白了");
            }
            else
            {
                await DisplayAlert("非法昵称", "请输入合法昵称", "明白了");
            }
        }
    }

    private async void MailEditor()
    {
        var newUserProperty = await DisplayPromptAsync("修改", "请输入您要修改的邮箱", "确定", "取消");
        bool flag = newUserProperty.IsValidEmail();
        if (flag)
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.Mail, newUserProperty, this);
            await DisplayAlert("修改成功", "内容合法", "明白了");
        }
        else
        {
            await DisplayAlert("邮箱无效", "请输入有效的邮箱地址", "明白了");
        }
    }

    private async void UserPropertyEditorButton_Click(object sender, EventArgs e)
    {
        Button button1 = (Button)sender;
        string InformationToBeModified = button1.ClassId;
        switch (InformationToBeModified)
        {
            case "UserNameEditor":
                UserNameEditor();
                break;
            case "PasswordEditor":
                if (modifyAuthentication)
                    await Shell.Current.GoToAsync(nameof(UserPasswordEditorPage));
                else
                    ModifyAuthentication();
                break;
            case "MailEditor":
                if (modifyAuthentication)
                    /*await Shell.Current.GoToAsync(nameof(UserMailEditorPage));*/
                    await DisplayAlert("1", "写完请将改行替换为注释内容", "3");
                else
                    ModifyAuthentication();
                break;
            case "TelEditor":
                //if (modifyAuthentication)
                await PopupAction.DisplayPopup(new UserTelEditPopup(this));
                //else
                //ModifyAuthentication();
                break;
            default:
                await DisplayAlert("抱歉", "出现异常，点击失败", "确定");
                break;
        }
    }
}