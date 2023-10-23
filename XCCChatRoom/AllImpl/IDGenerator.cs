using System.Text;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.AllImpl
{
    public class IDGenerator
    {
        public static async Task<string> GetCorrectUserUID(XFEExecuter XFEExecuter)
        {
            while (true)
            {
                var userUid = SummonRandomID(10);
                var result = await XFEExecuter.ExecuteGet<XFEChatRoom_UserInfoForm>(x => x.ID == userUid);
                if (result.Count == 0)
                {
                    return userUid;
                }
            }
        }
        public static async Task<string> GetCorrectPostID(XFEExecuter XFEExecuter)
        {
            while (true)
            {
                var postId = SummonRandomID(16);
                var result = await XFEExecuter.ExecuteGet<XFEChatRoom_CommunityPost>(x => x.PostID == postId);
                if (result.Count == 0)
                {
                    return postId;
                }
            }
        }
        public static async Task<string> GetCorrectCommentId(XFEExecuter XFEExecuter)
        {
            while (true)
            {
                var commentId = Guid.NewGuid().ToString();
                var result = await XFEExecuter.ExecuteGet<XFEChatRoom_CommunityComment>(x => x.CommentID == commentId);
                if (result.Count == 0)
                {
                    return commentId;
                }
            }
        }
        public static string SummonRandomID(int length)
        {
            var random = new Random();
            var result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                result.Append(random.Next(0, 10).ToString());
            }
            return result.ToString();
        }
    }
}
