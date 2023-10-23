namespace XCCChatRoom.AllImpl
{
    public class ControlExtension
    {
        public static async void BorderShake(Border border)
        {
            await border.TranslateTo(-10, 0, 50, Easing.CubicOut);
            await border.TranslateTo(10, 0, 50, Easing.CubicOut);
            await border.TranslateTo(-10, 0, 50, Easing.CubicOut);
            await border.TranslateTo(10, 0, 50, Easing.CubicOut);
            await border.TranslateTo(0, 0, 50, Easing.CubicOut);
        }
    }
}
