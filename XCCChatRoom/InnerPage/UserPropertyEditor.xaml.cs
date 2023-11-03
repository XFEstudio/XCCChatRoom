using XCCChatRoom.AllImpl;
using XFE各类拓展.StringExtension;

namespace XCCChatRoom.InnerPage;

public partial class UserPropertyEditor : ContentPage
{
    public UserPropertyEditor()
    {
        InitializeComponent();
    }


    private async void UserNameEditor()
    {
        string newUserProperty = await DisplayPromptAsync("修改", "请输入您要修改的昵称", "确定", "取消");
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

    private async void PasswordEditor()
    {
        string newUserProperty = await DisplayPromptAsync("修改", "请输入您要修改的密码", "确定", "取消");
        bool flag = newUserProperty.PasswordEditor();
        if (flag)
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.Password, newUserProperty, this);
            await DisplayAlert("修改成功", "内容合法", "明白了");
        }
        else
        {
            await DisplayAlert("密码过短或过长", "请输入合适长度的密码", "明白了");
        }
    }

    private async void MailEditor()
    {
        var newUserProperty = await DisplayPromptAsync("修改", "请输入您要修改的邮箱", "确定", "取消");
        bool flag = XFE各类拓展.StringExtension.StringExtension.IsValidEmail(newUserProperty);
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

    private async void TelEditor()
    {
        var newUserProperty = await DisplayPromptAsync("修改", "请输入您要修改的手机号", "确定", "取消");
        bool flag = newUserProperty.IsMobPhoneNumber();
        if (flag)
        {
            UserInfo.EditUserProperty(UserPropertyToEdit.PhoneNum, newUserProperty, this);
            await DisplayAlert("修改成功", "内容合法", "明白了");
        }
        else
        {
            await DisplayAlert("手机号无效", "请输入有效的手机号（目前仅支持中国大陆用户）", "明白了");
        }
    }
    private void UserPropertyEditorButton_Click(object sender, EventArgs e)
    {
        Button button1 = (Button)sender;
        string InformationToBeModified = button1.Text;
        switch (InformationToBeModified)
        {
            case "修改用户名":
                UserNameEditor();
                break;
            case "重置密码":
                Shell.Current.GoToAsync(nameof(UserPasswordEditorPage));
                /*PasswordEditor();*/
                break;
            case "重新绑定邮箱":
                MailEditor();
                break;
            case "重新绑定电话号码":
                TelEditor();
                Shell.Current.GoToAsync(nameof(UserTelEditorPage));
                
                break;
        }
    }
}