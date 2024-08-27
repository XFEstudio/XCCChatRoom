using HeyRed.MarkdownSharp;
using XFEExtension.NetCore.StringExtension;

namespace XCCChatRoom.ViewPage;

internal class LabelAndMessageId
{
    public Label label;
    public string messageId;
    public bool completeState = false;
    private string text = string.Empty;

    public string Text
    {
        get { return text; }
        set { text = value; SetTextInMarkDownForm(); }
    }

    public void InsertText(string text) => Text = Text.Insert(Text.Length - 1, text);
    private void SetTextInMarkDownForm()
    {
        var markdown = new Markdown();
        var htmlText = markdown.Transform(Text).Replace("\n", "<br>\n").Replace(" ", "&nbsp; ");
        label.Text = htmlText;
        htmlText.XL();
    }
    public LabelAndMessageId(Label label, string messageId)
    {
        this.label = label;
        this.messageId = messageId;
    }
    public LabelAndMessageId(string messageId)
    {
        this.messageId = messageId;
    }
}