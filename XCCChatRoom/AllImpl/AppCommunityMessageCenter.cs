using Timer = System.Timers.Timer;

namespace XCCChatRoom.AllImpl
{
    public class AppCommunityMessageCenter
    {
        public event EventHandler<CommunityMessage> CommunityMessageReceived;
        public AppCommunityMessageCenter()
        {
            var messageRefreshTimer = new Timer(30000);
            messageRefreshTimer.Elapsed += MessageRefreshTimer_Elapsed;
        }

        private void MessageRefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
        }
    }
    public class CommunityMessage : EventArgs
    {

    }
}
