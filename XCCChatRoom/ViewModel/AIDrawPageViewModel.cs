using CommunityToolkit.Mvvm.ComponentModel;
using XCCChatRoom.ViewPage;

namespace XCCChatRoom.ViewModel;

public partial class AIDrawPageViewModel(AIDrawPage viewPage) : ObservableObject
{
    public AIDrawPage ViewPage { get; init; } = viewPage;
}
