using XFEExtension.NetCore.ObjectExtension;
using XFE¸÷ÀàÍØÕ¹.NetCore.XFEDataBase;

namespace XCCChatRoom.Controls;

public partial class CommentCardView : ContentView
{
    public static readonly BindableProperty UserNameProperty = BindableProperty.Create(nameof(UserName), typeof(string), typeof(CommentCardView), string.Empty);
    public static readonly BindableProperty QuoteContentProperty = BindableProperty.Create(nameof(QuoteContent), typeof(string), typeof(CommentCardView), string.Empty);
    public static readonly BindableProperty CommentContentProperty = BindableProperty.Create(nameof(CommentContent), typeof(string), typeof(CommentCardView), string.Empty);
    public static readonly BindableProperty CommentTimeProperty = BindableProperty.Create(nameof(CommentTime), typeof(string), typeof(CommentCardView), string.Empty);
    public static readonly BindableProperty LikeCountProperty = BindableProperty.Create(nameof(LikeCount), typeof(int), typeof(CommentCardView), 0);
    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }
    public string QuoteContent
    {
        get => (string)GetValue(QuoteContentProperty);
        set => SetValue(QuoteContentProperty, value);
    }
    public string CommentContent
    {
        get => (string)GetValue(CommentContentProperty);
        set => SetValue(CommentContentProperty, value);
    }
    public string CommentTime
    {
        get => (string)GetValue(CommentTimeProperty);
        set => SetValue(CommentTimeProperty, value);
    }
    public int LikeCount
    {
        get => (int)GetValue(LikeCountProperty);
        set => SetValue(LikeCountProperty, value);
    }
    public bool IsLike
    {
        get => LikeButton.IsLike;
        set
        {
            LikeButton.IsLike = value;
        }
    }
    public string CommentID { get; set; }
    public XFEChatRoom_CommunityComment CurrentCommentData { get; set; }
    public event EventHandler<CommentCardLikeClickEventArgs> LikeClick;
    public event EventHandler<CommentCardQuoteClickEventArgs> QuoteClick;
    public event EventHandler CommentCardTapped;
    public CommentCardView(XFEChatRoom_CommunityComment commentEntity, XFEChatRoom_CommunityComment commentQuoteEntity)
    {
        InitializeComponent();
        this.BindingContext = this;
        CurrentCommentData = commentEntity;
        UserName = commentEntity.UName;
        CommentContent = commentEntity.CommentContent;
        LikeCount = commentEntity.CommentLike;
        CommentID = commentEntity.CommentID;
        CommentTime = commentEntity.CommentTime.ToString();
        if (commentQuoteEntity is not null)
        {
            QuoteContent = $"{commentQuoteEntity.UName}£º{commentQuoteEntity.CommentContent}";
            quoteBorder.IsVisible = true;
        }
        else
        {
            quoteBorder.IsVisible = false;
        }
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