using CommunityToolkit.Mvvm.ComponentModel;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom.ViewModel;

internal partial class UserPasswordEditorPageViewModel : ObservableObject
{
    public UserPasswordEditorPage ViewPage { get; init; }

    public UserPasswordEditorPageViewModel(UserPasswordEditorPage viewPage) => ViewPage = viewPage;
}
