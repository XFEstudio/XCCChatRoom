using CommunityToolkit.Mvvm.ComponentModel;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom.ViewModel;

internal partial class AIDrawPageViewModel(AIDrawPage viewPage) : ObservableObject
{
    public AIDrawPage ViewPage { get; init; } = viewPage;
}
