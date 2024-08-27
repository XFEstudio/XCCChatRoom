using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XApp.Core;

namespace XCCChatRoom.Model;

public class XAppInformationView(XAppInformation appInformation, ImageSource? appIcon, string appId, string appName, Version appVersion, string? appDescription, string author, string? company)
{
    public XAppInformation AppInformation { get; set; } = appInformation;
    public ImageSource AppIcon { get; init; } = appIcon;
    public string AppID { get; init; } = appId;
    public string AppName { get; init; } = appName;
    public Version AppVersion { get; init; } = appVersion;
    public string? AppDescription { get; init; } = appDescription == string.Empty ? null : appDescription;
    public string? Author { get; init; } = author == string.Empty ? null : author;
    public string? Company { get; init; } = company == string.Empty ? null : company;
    public static XAppInformationView LoadFromXAppInformation(XAppInformation appInformation) => new(appInformation, appInformation.AppIcon.IsNullOrEmpty() ? "xapp_light" : ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(appInformation.AppIcon))), appInformation.AppID, appInformation.AppName, appInformation.AppVersion, appInformation.AppDescription, appInformation.Author, appInformation.Company);
}
