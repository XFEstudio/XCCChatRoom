using XCCChatRoom.AllImpl;
using XFE各类拓展.StringExtension;

namespace XCCChatRoom.InnerPage;

public partial class UserPropertyEditPage : ContentPage
{
    public UserPropertyEditPage()
    {
        InitializeComponent();
    }
    private static bool modifyAuthentication = false;
    private async void ModifyAuthentication()
    {
        bool flag1 = false;
        bool flag2 = true;
        var randomCode = new Random().Next(100000, 999999).ToString();
        if (!modifyAuthentication)
        {
            bool telFlag = await DisplayAlert("当前环境不安全", "请进行身份验证", "确定", "取消");
            if (telFlag)
            {
                flag1 = true;
                if (flag2)
                {
                    await TencentSms.SendVerifyCode("1922760", "+86" + UserInfo.CurrentUser.Atel, new string[] { randomCode });
                    flag2 = false;
                }
            }
            else { flag1 = false; }
            if (flag1)
            {
                string captcha = await DisplayPromptAsync("手机号验证", "请输入验证码", "确定", "取消");
                if (captcha == randomCode)
                {
                    modifyAuthentication = true;
                }
            }
        }

    }
    private async void UserNameEditor()
    {
        string newUserProperty = await DisplayPromptAsync("修改", "请输入您要修改的昵称", "确定", "取消");
        if (newUserProperty is not null && newUserProperty != string.Empty)
        {
            bool flag = newUserProperty.UserNameEditor();
            if (flag)
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.UserName, newUserProperty, this);
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
        string InformationToBeModified = button1.Text;
        switch (InformationToBeModified)
        {
            case "修改用户名":
                UserNameEditor();
                break;
            case "重置密码":
                if (modifyAuthentication)
                    await Shell.Current.GoToAsync(nameof(UserPasswordEditorPage));
                else
                    ModifyAuthentication();
                break;
            case "重新绑定邮箱":

                if (modifyAuthentication)
                    /*await Shell.Current.GoToAsync(nameof(UserMailEditorPage));*/
                    await DisplayAlert("1", "写完请将改行替换为注释内容", "3");
                else
                    ModifyAuthentication();
                break;
            case "重新绑定电话号码":
                //if (modifyAuthentication)
                    await Shell.Current.GoToAsync(nameof(UserTelEditPage));
                //else
                    //ModifyAuthentication();
                break;
        }
    }
}