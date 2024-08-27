using CommunityToolkit.Mvvm.ComponentModel;
using XCCChatRoom.Controls.ViewModel;
using XFEExtension.NetCore.ObjectExtension;
using XFE¸÷ÀàÍØÕ¹.NetCore.XFEDataBase;

namespace XCCChatRoom.Controls;

public partial class CommentCardView : ContentView
{
    public string UserNameFirstLetter { get => ViewModel.UserNameFirstLetter; set => ViewModel.UserNameFirstLetter = value; }
    public string UserName { get => ViewModel.UserName; set => ViewModel.UserName = value; }
    public string QuoteContent { get => ViewModel.QuoteContent; set => ViewModel.QuoteContent = value; }
    public string CommentContent { get => ViewModel.CommentContent; set => ViewModel.CommentContent = value; }
    public string CommentTime { get => ViewModel.CommentTime; set => ViewModel.CommentTime = value; }
    public int LikeCount { get => ViewModel.LikeCount; set => ViewModel.LikeCount = value; }
    public bool IsLike
    {
        get => likeButton.IsLike;
        set
        {
            likeButton.IsLike = value;
        }
    }
    public string CommentID { get; set; }
    public XFEChatRoom_CommunityComment CurrentCommentData { get; set; }
    public event EventHandler<CommentCardLikeClickEventArgs> LikeClick;
    public event EventHandler<CommentCardQuoteClickEventArgs> QuoteClick;
    public event EventHandler CommentCardTapped;
    internal CommentCardViewViewModel ViewModel { get; init; }
    public CommentCardView(XFEChatRoom_CommunityComment commentEntity, XFEChatRoom_CommunityComment commentQuoteEntity)
    {
        InitializeComponent();
        CurrentCommentData = commentEntity;
        ViewModel = new(this, commentEntity, commentQuoteEntity);
        BindingContext = ViewModel;
    }

    public void ReloadData(XFEChatRoom_CommunityComment xFEChatRoom_CommunityComment, XFEChatRoom_CommunityComment xFEChatRoom_CommunityComment1)
    {
        if (CurrentCommentData.AboutEqual(xFEChatRoom_CommunityComment))
        {
            this.Dispatcher.Dispatch(() =>
            {
                CurrentCommentData = xFEChatRoom_CommunityComment;
                CommentContent = xFEChatRoom_CommunityComment.CommentContent;
                CommentID = xFEChatRoom_CommunityComment.CommentID;
                LikeCount = xFEChatRoom_CommunityComment.CommentLike;
                if (xFEChatRoom_CommunityComment1 is not null)
                {
                    QuoteContent = $"{xFEChatRoom_CommunityComment1.UName}£º{xFEChatRoom_CommunityComment1.CommentContent}";
                    quoteBorder.IsVisible = true;
                }
                else
                {
                    quoteBorder.IsVisible = false;
                }
            });
        }
    }

    private void LikeButton_Clicked(object sender, EventArgs e)
    {
        if (IsLike)
        {
            CurrentCommentData.CommentLike++;
            LikeCount++;
        }
        else
        {
            CurrentCommentData.CommentLike--;
            LikeCount--;
        }
        LikeClick?.Invoke(this, new CommentCardLikeClickEventArgs(CurrentCommentData));
    }

    private void QuoteButton_Clicked(object sender, EventArgs e)
    {
        QuoteClick?.Invoke(this, new CommentCardQuoteClickEventArgs(CommentID));
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        CommentCardTapped?.Invoke(this, EventArgs.Empty);
    }
}
public class CommentCardLikeClickEventArgs(XFEChatRoom_CommunityComment currentCommentEntity) : EventArgs
{
    public XFEChatRoom_CommunityComment CurrentCommentEntity { get; set; } = currentCommentEntity;
}
public class CommentCardQuoteClickEventArgs(string commentID) : EventArgs
{
    public string CommentID { get; set; } = commentID;
}