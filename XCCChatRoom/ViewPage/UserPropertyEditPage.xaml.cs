using MauiPopup;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Model;
using XCCChatRoom.ViewModel;
using XFEExtension.NetCore.StringExtension;

namespace XCCChatRoom.ViewPage;

public partial class UserPropertyEditPage : ContentPage
{
    internal UserPropertyEditPageViewModel ViewModel { get; init; }
    public UserPropertyEditPage()
    {
        InitializeComponent();
        ViewModel = new(this);
        BindingContext = ViewModel;
    }
}