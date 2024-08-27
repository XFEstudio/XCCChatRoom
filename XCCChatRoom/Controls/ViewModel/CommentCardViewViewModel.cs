using CommunityToolkit.Mvvm.ComponentModel;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.Controls.ViewModel;

internal partial class CommentCardViewViewModel : ObservableObject
{
    [ObservableProperty]
    private string userNameFirstLetter;
    [ObservableProperty]
    private string userName;
    [ObservableProperty]
    private string quoteContent;
    [ObservableProperty]
    private string commentContent;
    [ObservableProperty]
    private string commentTime;
    [ObservableProperty]
    private int likeCount;
    public CommentCardView ViewPage { get; init; }
    partial void OnUserNameChanged(string value) => UserNameFirstLetter = UserName[0].ToString() ?? "?";
    public CommentCardViewViewModel(CommentCardView viewPage, XFEChatRoom_CommunityComment commentEntity, XFEChatRoom_CommunityComment commentQuoteEntity)
    {
        ViewPage = viewPage;
        UserName = commentEntity.UName;
        CommentContent = commentEntity.CommentContent;
        LikeCount = commentEntity.CommentLike;
        ViewPage.CommentID = commentEntity.CommentID;
        CommentTime = commentEntity.CommentTime.ToString();
        if (commentQuoteEntity is not null)
        {
            QuoteContent = $"{commentQuoteEntity.UName}：{commentQuoteEntity.CommentContent}";
            ViewPage.quoteBorder.IsVisible = true;
        }
        else
        {
            ViewPage.quoteBorder.IsVisible = false;
        }
    }
}
