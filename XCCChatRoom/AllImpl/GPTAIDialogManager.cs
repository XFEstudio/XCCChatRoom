using System.Diagnostics;
using XFEExtension.NetCore.ArrayExtension;
using XFEExtension.NetCore.FileExtension;
using XFEExtension.NetCore.FormatExtension;
using XFEExtension.NetCore.XFEChatGPT;
using XFEExtension.NetCore.XFEChatGPT.ChatGPTInnerClass.HelperClass;

namespace XCCChatRoom.AllImpl
{
    public static class GPTAIDialogManager
    {
        public static string Suggestion { get; set; }
        public static event EventHandler<string> SuggestionsReceived;
        public static XFEMultiDictionary XFEEntries { get; set; } = new XFEMultiDictionary();
        public static MemorableXFEChatGPT MemorableXFEChatGPT { get; set; } = new MemorableXFEChatGPT();
        public static void AddDialog(string dialogId, string title, string system, string GPTVersion, string[] content)
        {
            XFEEntries.Add(dialogId, new XFEDictionary(new string[] { "Title", title, "System", system, "GPTVersion", GPTVersion, "Content", content.ToXFEString() }).ToString());
            SaveDialogs();
        }
        public static XFEDictionary GetDialog(string dialogId)
        {
            var result = XFEEntries[dialogId];
            if (result is not null)
                return new XFEDictionary(XFEEntries[dialogId]);
            else
                return null;
        }
        public static void SaveDialogs()
        {
            XFEEntries.ToString().WriteIn(AppPath.GPTAIDialogsPath);
        }
        public static void LoadDialogs()
        {
            if (AppPath.GPTAIDialogsPath.ReadOut(out string dialogsString))
            {
                try
                {
                    Trace.WriteLine(dialogsString);
                    XFEEntries = new XFEMultiDictionary(dialogsString);
                    foreach (var entry in XFEEntries)
                    {
                        MemorableXFEChatGPT.CreateDialog(entry.Header, new XFEDictionary(entry.Content)["System"], true, true);
                        MemorableXFEChatGPT.InsertDialogAutoComplete(entry.Header, new XFEDictionary(entry.Content)["Content"].ToXFEArray<string>());
                    }
                }
                catch (Exception ex)
                {
                    //PopupAction.DisplayPopup(new ErrorPopup("加载失败", $"加载对话时发生错误：\n{ex.Message}", 10));
                    Trace.WriteLine($"发生错误：{ex}");
                    Clipboard.SetTextAsync($"报错信息：{ex}\n\n报错时读取的内容：{dialogsString}");
                    XFEEntries = [];
                }
            }
        }
        public static void GetSuggestionFromGPT()
        {
            var uid = Guid.NewGuid().ToString();
            MemorableXFEChatGPT memorableXFEChatGPT = new MemorableXFEChatGPT();
            memorableXFEChatGPT.XFEChatGPTMessageReceived += MemorableXFEChatGPT_XFEChatGPTMessageReceived;
            memorableXFEChatGPT.CreateDialog(uid, "你是一个由寰宇朽力网络科技开发的对接ChatGPT的人工智能AI，目前仅支持文本聊天", true, true);
            memorableXFEChatGPT.InsertDialog(uid, new string[] { "请给出一些根据你的功能的用户提问示例，每个建议使用以下格式：|建议内容1|建议内容2|建议内容3|建议内容4|建议内容5|建议内容6，除此之外不要有任何其它文本，无需带标点符号", "|给我一篇随笔文章|你可以教我计算机网络技术吗|你会干什么|你可以告诉我怎么写随笔吗", "再回答2个", "|你可以解释一下学科中的复杂概念吗|你可以进行单位换算吗" });
            memorableXFEChatGPT.AskChatGPT(uid, Guid.NewGuid().ToString(), "再回答6个");
        }
        private static void MemorableXFEChatGPT_XFEChatGPTMessageReceived(object sender, MemorableGPTMessageReceivedEventArgs e)
        {
            switch (e.GenerateState)
            {
                case GenerateState.Start:
                    break;
                case GenerateState.Continue:
                    SuggestionsReceived?.Invoke(null, e.Message);
                    Suggestion += e.Message;
                    break;
                case GenerateState.End:
                    break;
                case GenerateState.Error:
                    Trace.WriteLine(e.Message);
                    break;
                default:
                    ProcessException.ShowEnumException();
                    break;
            }
        }
    }
}
