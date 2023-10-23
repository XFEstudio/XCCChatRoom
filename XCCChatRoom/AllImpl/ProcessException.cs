namespace XCCChatRoom.AllImpl
{
    public static class ProcessException
    {
        public static async void ShowEnumException()
        {
            await Shell.Current.DisplayAlert("枚举异常", "枚举类型能报错？？？？\n这合理吗？", "啊？");
        }
    }
}
