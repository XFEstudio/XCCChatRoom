using TencentCloud.Common;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;
using XFE各类拓展.TaskExtension;
using XFE各类拓展.WebExtension;
using XFE各类拓展.XEAEncryption;

namespace XCCChatRoom.AllImpl
{
    public class TencentSms
    {
        private static string _decodeID;
        public static string DecodeID
        {
            get { return _decodeID; }
            set
            {
                _decodeID = value.XEADecrypt("早期测试版ID")[..^2];
            }
        }
        private static string _decodeKey;
        public static string DecodeKey
        {
            get { return _decodeKey; }
            set
            {
                _decodeKey = value.XEADecrypt("早期测试版Key")[..^2];
            }
        }
        public static async Task<SendSmsResponse> SendVerifyCode(string templateId, string phoneNum, string[] args)
        {
            await new Action(() =>
            {
                while (DecodeID is null || DecodeKey is null) { }
            }).StartNewTask();
            try
            {
                Credential cred = new Credential
                {
                    SecretId = DecodeID,
                    SecretKey = DecodeKey
                };
                SmsClient client = new SmsClient(cred, "ap-guangzhou");
                SendSmsRequest req = new SendSmsRequest();
                req.SmsSdkAppId = "1400854601";
                req.SignName = "武汉寰宇朽力网络科技";
                req.TemplateId = templateId;
                req.TemplateParamSet = args;
                req.PhoneNumberSet = [phoneNum];
                return await client.SendSms(req);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
                return null;
            }
        }
        public static async void Initialize()
        {
            try
            {
                DecodeID = await "https://www.xfegzs.com/XFEChatRoom/SID.xfe".GetFromURLAsync();
                DecodeKey = await "https://www.xfegzs.com/XFEChatRoom/SKey.xfe".GetFromURLAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("错误", $"获取服务器信息失败！\n{ex.Message}", "确定");
            }
        }
    }
}
