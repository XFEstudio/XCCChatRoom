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
        if(newUserProperty is not null && newUserProperty != string.Empty)
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
                await Shell.Current.GoToAsync(nameof(UserPasswordEditorPage));
                break;
            case "重新绑定邮箱":
                /*await Shell.Current.GoToAsync(nameof(UserMailEditorPage));*/
                await DisplayAlert("1", "2", "3");
                break;
            case "重新绑定电话号码":
                await Shell.Current.GoToAsync(nameof(UserTelEditorPage));
                break;
        }
    }
}