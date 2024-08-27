using XFEExtension.NetCore.ArrayExtension;
using XFEExtension.NetCore.ObjectExtension;
using XFEExtension.NetCore.TaskExtension;
using XFE¸÷ÀàÍØÕ¹.NetCore.XFEDataBase;

namespace XCCChatRoom.Controls;

public partial class PostCardView : ContentView
{
    public static readonly BindableProperty PostTitleProperty = BindableProperty.Create(nameof(PostTitle), typeof(string), typeof(PostCardView), "PostTitle");
    public static readonly BindableProperty PostContentProperty = BindableProperty.Create(nameof(PostContent), typeof(string), typeof(PostCardView), "PostContent");
    public static readonly BindableProperty PostAuthorProperty = BindableProperty.Create(nameof(PostAuthor), typeof(string), typeof(PostCardView), "PostAuthor");
    public static readonly BindableProperty PostTimeProperty = BindableProperty.Create(nameof(PostTime), typeof(string), typeof(PostCardView), "PostTime");
    public static readonly BindableProperty PostIdProperty = BindableProperty.Create(nameof(PostId), typeof(string), typeof(PostCardView), "PostId");
    public static readonly BindableProperty LikeCountProperty = BindableProperty.Create(nameof(LikeCount), typeof(int), typeof(PostCardView), 0);
    public string PostTitle
    {
        get => (string)GetValue(PostTitleProperty);
        set => SetValue(PostTitleProperty, value);
    }
    public string PostContent
    {
        get => (string)GetValue(PostContentProperty);
        set => SetValue(PostContentProperty, value);
    }
    public string PostAuthor
    {
        get => (string)GetValue(PostAuthorProperty);
        set => SetValue(PostAuthorProperty, value);
    }
    public string PostTime
    {
        get => (string)GetValue(PostTimeProperty);
        set => SetValue(PostTimeProperty, value);
    }
    public string PostId
    {
        get => (string)GetValue(PostIdProperty);
        set => SetValue(PostIdProperty, value);
    }
    public int LikeCount
    {
        get => (int)GetValue(LikeCountProperty);
        set => SetValue(LikeCountProperty, value);
    }
    private string[] tags;
    public string[] Tags
    {
        get => tags;
        set
        {
            tags = value;
            tagStackLayout.Clear();
            if (value.Length == 0)
            {
                tagBoxView.IsVisible = false;
            }
            else
            {
                foreach (var tagString in value)
                {
                    if (!string.IsNullOrEmpty(tagString))
                    {
                        var tagButton = new PostTagView
                        {
                            TagText = $"#{tagString}",
                            Margin = new Thickness(0, 0, 10, 0)
                        };
                        tagButton.Clicked += TagButton_Clicked;
                        tagStackLayout.Children.Add(tagButton);
                    }
                }
            }
        }
    }
    private XFEChatRoom_CommunityPost postEntity;
    public XFEChatRoom_CommunityPost PostEntity
    {
        get => postEntity;
        set
        {
            PostTitle = value.PostTitle;
            PostContent = value.PostContent;
            PostAuthor = value.UName;
            PostTime = value.PostTime.ToString();
            PostId = value.PostID;
            LikeCount = value.PostLike;
            Tags = value.PostTag.ToXFEArray<string>();
            postEntity = value;
        }
    }
    public event EventHandler<PostCardViewClickEventArgs> Click;
    public event EventHandler<PostCardViewLikeClickEventArgs> LikeClick;
    public event EventHandler<PostCardViewTagClickEventArgs> TagClick;
    private bool isLike;
    public bool IsLike
    {
        get { return isLike; }
        set
        {
            isLike = value;
            if (isLike)
                btnLike.IsLike = true;
            else
                btnLike.IsLike = false;
        }
    }

    public PostCardView(XFEChatRoom_CommunityPost xFEChatRoom_CommunityPost = null, bool showAnimation = true)
    {
        if (showAnimation)
        {
            this.TranslationY = -200;
            this.Scale = 0;
            this.Opacity = 0;
        }
        InitializeComponent();
        this.BindingContext = this;
        if (xFEChatRoom_CommunityPost is not null)
            PostEntity = xFEChatRoom_CommunityPost;
        if (showAnimation)
            new Action(() =>
            {
                this.TranslateTo(0, 0, 1000, Easing.CubicOut);
                this.ScaleTo(1, 1000, Easing.CubicOut);
                this.FadeTo(1, 1000, Easing.CubicOut);
            }).StartNewTask();
    }

    private void TagButton_Clicked(object sender, PostTagViewTagClickEventArgs e)
    {
        TagClick?.Invoke(this, new PostCardViewTagClickEventArgs(PostEntity, e.TagText[1..]));
    }

    public void ReloadData(XFEChatRoom_CommunityPost xFEChatRoom_CommunityPost)
    {
        if (!PostEntity.AboutEqual(xFEChatRoom_CommunityPost))
            PostEntity = xFEChatRoom_CommunityPost;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Click?.Invoke(this, new PostCardViewClickEventArgs(PostEntity, e));
    }

    private void BtnLike_Clicked(object sender, EventArgs e)
    {
        if (IsLike)
        {
            isLike = false;
            LikeCount--;
            LikeClick?.Invoke(this, new PostCardViewLikeClickEventArgs(PostEntity, false));
        }
        else
        {
            isLike = true;
            LikeCount++;
            LikeClick?.Invoke(this, new PostCardViewLikeClickEventArgs(PostEntity, true));
        }
    }
}

public class PostCardViewTagClickEventArgs(XFEChatRoom_CommunityPost postEntity, string tagString)
{
    public XFEChatRoom_CommunityPost PostEntity { get; init; } = postEntity;
    public string TagString { get; init; } = tagString;
}

public class PostCardViewClickEventArgs(XFEChatRoom_CommunityPost postEntity, TappedEventArgs tappedEventArgs) : EventArgs
{
    public XFEChatRoom_CommunityPost PostEntity { get; init; } = postEntity;
    public TappedEventArgs TappedEventArgs { get; init; } = tappedEventArgs;
}
public class PostCardViewLikeClickEventArgs(XFEChatRoom_CommunityPost postEntity, bool isLike) : EventArgs
{
    public bool IsLike { get; init; } = isLike;
    public XFEChatRoom_CommunityPost PostEntity { get; init; } = postEntity;
}