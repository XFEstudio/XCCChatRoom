using MauiPopup;
using XCCChatRoom.Controls;
using XFE各类拓展.NetCore.XFEDataBase;

namespace XCCChatRoom.AllImpl
{
    public class AppAlgorithm
    {
        private static XFEExecuter XFEExecuter = XCCDataBase.XFEDataBase.CreateExecuter();
        public static async Task<List<XFEChatRoom_CommunityPost>> GetLatestPost(int count)
        {
            try
            {
                XFEExecuter.RefreshExecuter();
                var result = await XFEExecuter.ExecuteGet<XFEChatRoom_CommunityPost>(x => true);
                if (result.Count > count)
                {
                    return result.GetRange(0, count);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                await PopupAction.DisplayPopup(new ErrorPopup("刷新失败", $"无法获取帖子列表\n{ex.Message}"));
                return null;
            }
        }
        public static async Task<List<XFEChatRoom_CommunityPost>> GetElderPost(int count, List<string> postIdList)
        {
            try
            {
                XFEExecuter.RefreshExecuter();
                var result = await XFEExecuter.ExecuteGet<XFEChatRoom_CommunityPost>(x => !postIdList.Contains(x.PostID));
                if (result.Count > count)
                {
                    return result.GetRange(0, count);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                await PopupAction.DisplayPopup(new ErrorPopup("刷新失败", $"无法获取帖子列表\n{ex.Message}"));
                return null;
            }
        }
        public static async Task<List<XFEChatRoom_CommunityComment>> GetPostComment(string PostId, int count, List<string> loadedComments)
        {
            try
            {
                var result = await XFEExecuter.ExecuteGet<XFEChatRoom_CommunityComment>(x => x.PostID == PostId && !loadedComments.Contains(x.CommentID), y => y.FloorCount);
                if (result.Count > count)
                {
                    return result.GetRange(0, count);
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                await PopupAction.DisplayPopup(new ErrorPopup("刷新失败", $"无法获取评论列表\n{ex.Message}"));
                return null;
            }
        }
        public static async Task<int> GetCommentFloor(string CurrentPostID, XFEExecuter XFEExecuter)
        {
            XFEExecuter.RefreshExecuter();
            return (await XFEExecuter.ExecuteGet<XFEChatRoom_CommunityComment>(x => x.PostID == CurrentPostID)).Count + 1;
        }
    }
}
