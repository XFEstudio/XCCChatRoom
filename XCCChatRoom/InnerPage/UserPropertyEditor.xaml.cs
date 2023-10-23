using XCCChatRoom.AllImpl;

namespace XCCChatRoom.InnerPage;

public partial class UserPropertyEditor : ContentPage
{
    public UserPropertyEditor()
    {
        InitializeComponent();
    }

    private async void UserNameEditorButton_Click(object sender, TappedEventArgs e)
    {
        var checkParamName = e.Parameter as string;
        var paramName = string.Empty;
        switch (checkParamName)
        {
            case "修改用户名":
                paramName = "昵称";
                break;
            case "重置密码":
                paramName = "密码";
                break;
            case "重新绑定邮箱":
                paramName = "邮箱";
                break;
            case "重新绑定电话号码":
                paramName = "电话号码";
                break;
        }
        var newUserProperty = await DisplayPromptAsync($"修改{paramName}", $"请输入新的{paramName}", "确定", "取消");
        if (newUserProperty is not null && newUserProperty != string.Empty)
        {
            if (newUserProperty.Contains(' ') && newUserProperty.VerifyString())
            {
                UserInfo.EditUserProperty(UserPropertyToEdit.UserName, newUserProperty, this);
            }
            else
            {
                await DisplayAlert("出错啦！", $"请输入不含空格的{paramName}", "明白了");
            }
        }
        else
        {
            await DisplayAlert("出错啦！", $"{paramName}不能为空", "明白了");
        }
    }
}