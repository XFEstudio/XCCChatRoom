using XCCChatRoom.Profiles;

namespace XCCChatRoom.AllImpl;

public static class ThemeImageGetter
{
    private static List<View> imageList = [];
    public static void Add(params Image[] image)
    {
        imageList.AddRange(image);
        ThemeChanged(SystemProfile.Theme);
    }

    public static void Add(params ImageButton[] imageButton)
    {
        imageList.AddRange(imageButton);
        ThemeChanged(SystemProfile.Theme);
    }

    public static void ThemeChanged(AppTheme appTheme)
    {
        foreach (var control in imageList)
        {
            if (control is ImageButton || control is Image)
            {
                control.Dispatcher.Dispatch(() =>
                {
                    dynamic imageControl = control;
                    control.ClassId = ChangeClassIdByTheme(control.ClassId, appTheme);
                    imageControl.Source = imageControl.ClassId;
                });
            }
        }
    }
    private static string ChangeClassIdByTheme(string classId, AppTheme appTheme) => appTheme switch
    {
        AppTheme.Light => classId.Replace("dark", "light"),
        AppTheme.Dark => classId.Replace("light", "dark"),
        _ => ChangeClassIdByTheme(classId, Application.Current!.RequestedTheme),
    };
}
