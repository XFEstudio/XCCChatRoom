using CommunityToolkit.Mvvm.ComponentModel;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom.ViewModel;

internal partial class XEAEncryptPageViewModel(XEAEncryptPage viewPage) : ObservableObject
{
    public XEAEncryptPage ViewPage { get; init; } = viewPage;
}
