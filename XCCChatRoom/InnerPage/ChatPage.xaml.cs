using Microsoft.Maui.Controls.Shapes;
#if ANDROID
using Android.Media;
using Plugin.LocalNotification;
#endif
using XFE各类拓展.NetCore.CyberComm.XCCNetWork;
using XFE各类拓展.NetCore.TaskExtension;
using Image = Microsoft.Maui.Controls.Image;
using XCCChatRoom.AllImpl;
using XFE各类拓展.NetCore.StringExtension;
using MauiPopup;
using XCCChatRoom.Controls;
using XFE各类拓展.NetCore;

namespace XCCChatRoom.InnerPage;
[QueryProperty(nameof(GroupName), nameof(GroupName))]
[QueryProperty(nameof(CurrentName), nameof(CurrentName))]
public partial class ChatPage : ContentPage
{
    #region 公共属性
    public readonly BindableProperty GroupNameProperty = BindableProperty.Create(nameof(GroupName), typeof(string), typeof(ChatPage), string.Empty);
    public readonly BindableProperty DisplayGroupNameProperty = BindableProperty.Create(nameof(DisplayGroupName), typeof(string), typeof(ChatPage), string.Empty);
    public string DisplayGroupName
    {
        get => (string)GetValue(DisplayGroupNameProperty);
        set => SetValue(DisplayGroupNameProperty, value);
    }
    public string GroupName
    {
        get => (string)GetValue(GroupNameProperty);
        set
        {
            SetValue(GroupNameProperty, value);
            DisplayGroupName = $"群组|{value}";
        }
    }
    public static ChatPage CurrentInstance { get; private set; }
    private string currentName;
    public string CurrentName
    {
        get => currentName;
        set
        {
            currentName = value;
            StartGroupComm(GroupName, value);
        }
    }
    #endregion
    #region 字段
    private readonly List<ImageButton> emotionImageButtonList = [];
    private readonly List<Image> emotionImageList = [];
    private readonly XCCNetWork xCCNetWork;
    private XCCGroup xCCGroup;
    private readonly XCCMessageReceiveHelper messageReceiveHelper;
    private readonly Grid loadGrid;
    private readonly Image serverImg;
    private ImageButton lastButtonClicked = null;
    private readonly ToolbarItem phoneCallItem;
#if ANDROID
    private AudioRecord audioRecord;
    private AudioTrack audioTrack;
#endif
    private bool isRecording = false;
    private byte[] audioBuffer;
    private bool isPlaying = false;
    private bool connected = false;
    private bool firstConnect = true;
    private bool isScrolled = false;
    private bool phoneCallEnabled = false;
    private string lastSender = string.Empty;
    private int MessageCount = 0;
    #endregion
    public ChatPage()
    {
        InitializeComponent();
        CurrentInstance = this;
        BindingContext = this;
        backButton.Command = new Command(() =>
        {
            Shell.Current.SendBackButtonPressed();
        });
        phoneCallItem = new ToolbarItem
        {
            Text = "开启语音",
            IconImageSource = "phone",
            Command = new Command(async () =>
            {
                if (phoneCallEnabled)
                {
                    phoneCallItem.IconImageSource = "phone";
#if ANDROID
                    StopRecording();
                    StopPlayback();
#endif
                    phoneCallEnabled = false;
                }
                else
                {
                    PermissionStatus m_statusRequestStorageRead = await Permissions.RequestAsync<Permissions.Microphone>();
                    switch (m_statusRequestStorageRead)
                    {
                        case PermissionStatus.Granted:
                            phoneCallItem.IconImageSource = "phone_call";
#if ANDROID
                            StartRecording();
                            StartPlayback();
#endif
                            phoneCallEnabled = true;
                            break;
                        case PermissionStatus.Denied:
                            await DisplayAlert("请求拒绝", "无法使用麦克风进行聊天交流，请前往手机设置打开麦克风权限", "啊这，行吧");
                            break;
                        case PermissionStatus.Disabled:
                            await DisplayAlert("暂无权限", "无法使用麦克风进行聊天交流，可能没有麦克风设备", "彳亍吧");
                            break;
                        case PermissionStatus.Unknown:
                            await DisplayAlert("我 氵则", "请求处于未知状态？？？？？？？？？", "啊这（截图和我反馈）");
                            break;
                        default:
                            await DisplayAlert("这位更是个寄吧", "出现错误", "我测");
                            break;
                    }
                }
            })
        };
        ToolbarItems.Add(phoneCallItem);
        xCCNetWork = new XCCNetWork();
        messageReceiveHelper = new XCCMessageReceiveHelper(AppPath.ChatDialogHistoryPath, xCCNetWork);
        xCCNetWork.Connected += XCCNetWork_Connected;
        xCCNetWork.ConnectionClosed += XCCNetWork_ConnectionClosed;
        messageReceiveHelper.AudioBufferReceived += MessageReceiveHelper_AudioBufferReceived;
        messageReceiveHelper.TextReceived += MessageReceiveHelper_TextReceived;
        messageReceiveHelper.FileReceived += MessageReceiveHelper_FileReceived;
        messageReceiveHelper.ExceptionOccurred += MessageReceiveHelper_ExceptionOccurred;
#if ANDROID
        LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
#endif
        #region 加载动画
        var serverImg = new Image
        {
            Source = "server",
            WidthRequest = 15,
            HeightRequest = 15,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        var serverLabel = new Label
        {
            Text = "正在连接",
            TextColor = Color.FromArgb("#ECECF1"),
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        var serverLabel2 = new Label
        {
            Text = "...",
            TextColor = Color.FromArgb("#ECECF1"),
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        var loadGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(25, GridUnitType.Absolute) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
            },
            Margin = new Thickness(0, 5),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Children = { serverImg, serverLabel, serverLabel2 }
        };
        loadGrid.SetColumn(serverLabel, 0);
        loadGrid.SetColumn(serverImg, 1);
        loadGrid.SetColumn(serverLabel2, 2);
        ChatStack.Children.Add(loadGrid);
        this.loadGrid = loadGrid;
        this.serverImg = serverImg;
        loadGrid.IsVisible = false;
        new Action(async () =>
        {
            while (true)
            {
                if (this.serverImg is not null)
                {
                    await this.serverImg.ScaleYTo(1.6d, 80, Easing.BounceOut);
                    var task1 = this.serverImg.ScaleXTo(0.7d, 80, Easing.BounceOut);
                    await this.serverImg.ScaleYTo(1d, 50, Easing.BounceOut);
                    var task2 = this.serverImg.ScaleXTo(1d, 50, Easing.BounceOut);
                    Thread.Sleep(500);
                    await this.serverImg.ScaleXTo(1.6d, 80, Easing.BounceOut);
                    var task3 = this.serverImg.ScaleYTo(0.6d, 80, Easing.BounceOut);
                    await this.serverImg.ScaleXTo(1d, 50, Easing.BounceOut);
                    var task4 = this.serverImg.ScaleYTo(1d, 50, Easing.BounceOut);
                    Thread.Sleep(500);
                }
            }
        }).StartNewTask();
        #endregion
        #region 表情
        foreach (var emo in ChatEmotion.EmotionNameList)
        {
            var imgButton = new ImageButton
            {
                Source = emo,
                WidthRequest = 60,
                HeightRequest = 60,
                ClassId = emo
            };
            imgButton.Clicked += EmotionImageButtonButton_Clicked;
            emotionImageButtonList.Add(imgButton);
            var image = new Image
            {
                Source = emo,
                WidthRequest = 30,
                HeightRequest = 30,
                ClassId = emo
            };
            emotionImageList.Add(image);
        }
        #endregion
    }

    private async void MessageReceiveHelper_ExceptionOccurred(XFECyberCommException sender)
    {
        connected = false;
        ChatStack.Dispatcher.Dispatch(() =>
        {
            ChatStack.Dispatcher.Dispatch(() =>
            {
                var messageLabel = new Label
                {
                    Text = $"发生错误：{sender.Message}",
                    TextColor = Color.FromArgb("#F87171"),
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    LineBreakMode = LineBreakMode.WordWrap
                };
                var messageGrid = new Grid
                {
                    BackgroundColor = Color.FromArgb("#444654"),
                    Padding = new Thickness(20, 5, 5, 20),
                    Children = { messageLabel }
                };
                ChatStack.Children.Add(messageGrid);
            });
            Console.WriteLine(sender);
        });
        await PopupAction.DisplayPopup(new ErrorPopup("出现错误", sender.Message));
    }

    private async void MessageReceiveHelper_FileReceived(bool isHistory, XCCFile message)
    {
        switch (message.FileType)
        {
            case XCCFileType.Image:
                await ShowStandardImage(isHistory, message);
                break;
            case XCCFileType.Video:
                break;
            case XCCFileType.Audio:
                break;
            default:
                break;
        }
    }

    private async void MessageReceiveHelper_TextReceived(bool isHistory, XCCMessage message)
    {
        switch (message.MessageType)
        {
            case XCCTextMessageType.Text:
                if (isHistory)
                {
                    if (message.Message.Contains("[Emotion]"))
                    {
                        var image = message.Message.Replace("[Emotion]", string.Empty);
                        await ShowEmotion(message.Sender, image, false);
                    }
                    else
                    {
                        await ShowTextMessage(message.Sender, message.Message, false);
                    }
                }
                else
                {
#if ANDROID
                    var iconImg = new NotificationImage();
                    iconImg.ResourceName = @"C:\Users\XFEstudio\Desktop\work\C#\OtherProject\XCCChatRoom\XCCChatRoom\Resources\AppIcon\logoicon.png";
                    var request = new NotificationRequest
                    {
                        Title = DisplayGroupName,
                        Subtitle = $"新的群消息",
                        Description = $"{message.Sender}：{message.Message}",
                        BadgeNumber = MessageCount++,
                        Image = iconImg,

                    };
                    await LocalNotificationCenter.Current.Show(request);
#endif
                    if (message.Message.Contains("[Emotion]"))
                    {
                        var image = message.Message.Replace("[Emotion]", string.Empty);
                        await ShowEmotion(message.Sender, image);
                    }
                    else
                    {
                        await ShowTextMessage(message.Sender, message.Message);
                    }
                }
                //new Action(async () =>
                //{
                //    while (!isScrolled)
                //    {
                //        try
                //        {
                //            await ChatScrollView.ScrollToAsync(0, ChatStack.DesiredSize.Height, false);
                //        }
                //        catch { }
                //    }
                //}).StartNewTask();
                break;
            default:
                break;
        }
    }

    private void MessageReceiveHelper_AudioBufferReceived(byte[] sender)
    {
#if ANDROID
        if (phoneCallEnabled)
            PlayAudioData(sender);
#endif
    }

    private async void EmotionImageButtonButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (connected)
            {
                await xCCGroup.SendTextMessage($"[Emotion]{(sender as ImageButton).ClassId}");
                await ShowEmotion(CurrentName, (sender as ImageButton).ClassId);
            }
            else
            {
                await DisplayAlert("未连接到服务器", "请检查网络连接", "确定");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("发送消息失败", ex.Message, "确定");
        }
    }

    private async void SendSelectedImage(FileResult fileResult)
    {
        if (connected)
        {
            var imageView = await ShowImage(CurrentName, fileResult.FullPath, fileResult.FileName, false);
            if (await xCCGroup.SendImage(fileResult.FullPath))
            {
                imageView.IsVisible = true;
            }
            else
            {
                await PopupAction.DisplayPopup(new TipPopup("图片发送失败", "发送超时"));
            }
        }
        else
        {
            await DisplayAlert("未连接到服务器", "请检查网络连接", "确定");
        }
    }

    private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
    {
        MessageCount = 0;
    }
#if ANDROID
    public void StartRecording()
    {
        int sampleRate = 44100;
        ChannelIn channelConfig = ChannelIn.Mono;
        Encoding audioFormat = Encoding.Pcm16bit;
        int bufferSize = AudioRecord.GetMinBufferSize(sampleRate, channelConfig, audioFormat);
        audioBuffer = new byte[bufferSize];
        audioRecord = new AudioRecord(AudioSource.Mic, sampleRate, channelConfig, audioFormat, bufferSize);
        audioRecord.StartRecording();
        isRecording = true;
        new Thread(ReadAudio).Start();
    }

    public void StopRecording()
    {
        isRecording = false;
        audioRecord.Stop();
        audioRecord.Release();
    }

    private async void ReadAudio()
    {
        while (isRecording)
        {
            int bytesRead = await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);

            if (bytesRead > 0)
            {
                try
                {
                    await xCCGroup.SendBinaryMessage(audioBuffer);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("发送失败", ex.Message, "确定");
                }
            }
        }
    }

    public void StartPlayback()
    {
        int bufferSizeInBytes = AudioTrack.GetMinBufferSize(44100, ChannelOut.Mono, Encoding.Pcm16bit);
        var audioAttributes = new AudioAttributes.Builder()
            .SetUsage(AudioUsageKind.Media)
            .SetContentType(AudioContentType.Music)
            .Build();
        audioTrack = new AudioTrack(
            audioAttributes,
            new AudioFormat.Builder()
                .SetSampleRate(44100)
                .SetEncoding(Encoding.Pcm16bit)
                .SetChannelMask(ChannelOut.Mono)
                .Build(),
            bufferSizeInBytes,
            AudioTrackMode.Stream,
            0);
        audioTrack.Play();
        isPlaying = true;
    }

    public void StopPlayback()
    {
        isPlaying = false;
        audioTrack.Stop();
        audioTrack.Release();
    }

    public void PlayAudioData(byte[] audioData)
    {
        if (isPlaying && audioData is not null)
        {
            audioTrack.Write(audioData, 0, audioData.Length, WriteMode.NonBlocking);
        }
    }
#endif

    public async void StartGroupComm(string groupName, string senderName)
    {
        ShowConnectingTip();
        xCCGroup = xCCNetWork.CreateGroup(groupName, senderName);
        try
        {
            var task = xCCGroup.StartXCC(true, 50);
            await xCCGroup.WaitConnect();
            if (firstConnect)
            {
                try
                {
                    await messageReceiveHelper.LoadGroup(GroupName);
                    if (!await xCCGroup.GetHistory())
                    {
                        await PopupAction.DisplayPopup(new ErrorPopup("无法获取消息记录", "服务器返回校验错误"));
                    }
                    firstConnect = false;
                }
                catch (Exception ex)
                {
                    await PopupAction.DisplayPopup(new ErrorPopup("获取历史消息失败", ex.Message));
                }
            }
            connected = true;
            HideConnectingTip();
        }
        catch (Exception ex)
        {
            await DisplayAlert("网络错误", ex.Message, "确定");
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (phoneCallEnabled)
        {
            phoneCallItem.IconImageSource = "phone";
#if ANDROID
            StopRecording();
            StopPlayback();
#endif
            phoneCallEnabled = false;
        }
        StopGroupComm();
        return base.OnBackButtonPressed();
    }

    public void StopGroupComm()
    {
        if (connected)
        {
            xCCGroup.CloseXCC();
        }
    }

    public void ShowConnectingTip()
    {
        loadGrid.IsVisible = true;
    }

    public void HideConnectingTip()
    {
        loadGrid.IsVisible = false;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (InputEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    #region 显示消息
    public async Task ShowStandardImage(bool isHistory, XCCFile xCCFile)
    {
        await Console.Out.WriteLineAsync($"发送者：{xCCFile.Sender}，是否加载：{xCCFile.Loaded}，是否为历史：{isHistory}");
        if (xCCFile.Loaded)
        {
            await ShowImage(xCCFile.Sender, xCCFile.FileBuffer, xCCFile.MessageId, !isHistory);
        }
        else
        {
            var imageView = await ShowImage(xCCFile.Sender, xCCFile.FileBuffer, xCCFile.MessageId, !isHistory);
            xCCFile.FileLoaded += xCCFile =>
            {
                imageView.Source = ImageSource.FromStream(() => new MemoryStream(xCCFile.FileBuffer));
            };
        }
    }

    public async Task<Image> ShowImage(string name, byte[] buffer, string imageId, bool autoScroll = true)
    {
        Border imageBorder = null;
        ImageSource imageSource = buffer == null ? null : ImageSource.FromStream(() => new MemoryStream(buffer));
        Image imageView = null;
        if (name == CurrentName)
        {
            imageView = new Image
            {
                Source = imageSource,
                MinimumWidthRequest = 100,
                MinimumHeightRequest = 100,
                MaximumHeightRequest = 600,
                MaximumWidthRequest = 300,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            imageBorder = new Border
            {
                Stroke = Color.FromArgb("#202127"),
                StrokeThickness = 1,
                Padding = new Thickness(8, 5),
                BackgroundColor = Color.FromArgb("#444654"),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(5, 5, 5, 5)
                },
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Content = imageView
            };
        }
        else
        {
            imageView = new Image
            {
                Source = imageSource,
                MinimumWidthRequest = 100,
                MinimumHeightRequest = 100,
                MaximumHeightRequest = 600,
                MaximumWidthRequest = 300,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            imageBorder = new Border
            {
                Stroke = Color.FromArgb("#202127"),
                Margin = new Thickness(10, 0, 0, 0),
                StrokeThickness = 1,
                Padding = new Thickness(8, 5),
                BackgroundColor = Color.FromArgb("#343541"),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(5, 5, 5, 5)
                },
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Content = imageView
            };
        }
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += async (sender, e) =>
        {
            await PopupAction.DisplayPopup(new ImagePopup(buffer, imageId));
        };
        imageBorder.GestureRecognizers.Add(tapGestureRecognizer);
        await AppendSenderAndShowMessage(name, imageBorder, autoScroll);
        return imageView;
    }

    public async Task<Image> ShowImage(string name, string filePath, string imageId, bool showImage = true, bool autoScroll = true)
    {
        ImageSource imageSource = ImageSource.FromFile(filePath);
        Border imageBorder;
        Image imageView;
        if (name == CurrentName)
        {
            imageView = new Image
            {
                Source = imageSource,
                MinimumWidthRequest = 100,
                MinimumHeightRequest = 100,
                MaximumHeightRequest = 600,
                MaximumWidthRequest = 300,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = showImage
            };
            imageBorder = new Border
            {
                Stroke = Color.FromArgb("#202127"),
                StrokeThickness = 1,
                Padding = new Thickness(8, 5),
                BackgroundColor = Color.FromArgb("#444654"),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(5, 5, 5, 5)
                },
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Content = imageView
            };
        }
        else
        {
            imageView = new Image
            {
                Source = imageSource,
                MinimumWidthRequest = 100,
                MinimumHeightRequest = 100,
                MaximumHeightRequest = 600,
                MaximumWidthRequest = 300,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = showImage
            };
            imageBorder = new Border
            {
                Stroke = Color.FromArgb("#202127"),
                Margin = new Thickness(10, 0, 0, 0),
                StrokeThickness = 1,
                Padding = new Thickness(8, 5),
                BackgroundColor = Color.FromArgb("#343541"),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(5, 5, 5, 5)
                },
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Content = imageView
            };
        }
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += async (sender, e) =>
        {
            await PopupAction.DisplayPopup(new ImagePopup(await File.ReadAllBytesAsync(filePath), imageId));
        };
        imageBorder.GestureRecognizers.Add(tapGestureRecognizer);
        await AppendSenderAndShowMessage(name, imageBorder, autoScroll);
        return imageView;
    }

    public async Task ShowEmotion(string name, string image, bool autoScroll = true)
    {
        Image emotionImageView = null;
        if (name == CurrentName)
        {
            emotionImageView = new Image
            {
                Source = image,
                WidthRequest = 100,
                HeightRequest = 100,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                ClassId = image
            };
        }
        else
        {
            emotionImageView = new Image
            {
                Source = image,
                WidthRequest = 100,
                HeightRequest = 100,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                ClassId = image
            };
        }
        await AppendSenderAndShowMessage(name, emotionImageView, autoScroll);
    }

    public async Task ShowTextMessage(string name, string message, bool autoScroll = true)
    {
        var URLs = message.GetUrl();
        Label messageLabel = null;
        Border messageBorder = null;
        if (name == CurrentName)
        {
            messageLabel = new Label
            {
                Text = message,
                TextColor = Color.FromArgb("#ECECF1"),
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.WordWrap
            };
            messageBorder = new Border
            {
                Stroke = Color.FromArgb("#202127"),
                StrokeThickness = 1,
                Padding = new Thickness(8, 5),
                BackgroundColor = Color.FromArgb("#343541"),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(5, 5, 5, 5)
                },
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Content = messageLabel
            };
            if (URLs.Length > 0)
            {
                var urlFlexLayout = new FlexLayout
                {
                    Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
                };
                foreach (var url in URLs)
                {
                    var urlLabel = new Label
                    {
                        Text = url,
                        TextColor = Color.FromArgb("#ECECF1"),
                        FontSize = 16,
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Center,
                        LineBreakMode = LineBreakMode.WordWrap
                    };
                    var urlBorder = new Border
                    {
                        Stroke = Color.FromArgb("#202127"),
                        StrokeThickness = 1,
                        Padding = new Thickness(8, 5),
                        BackgroundColor = Color.FromArgb("#343541"),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = new CornerRadius(5, 5, 5, 5)
                        },
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Center,
                        Content = urlLabel
                    };
                    urlBorder.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () =>
                        {
                            await Clipboard.SetTextAsync(url);
                            await Launcher.OpenAsync(url);
                        })
                    });
                    urlFlexLayout.Children.Add(urlBorder);
                }
                var inBorderStackLayout = new StackLayout
                {
                    Children = { messageLabel, urlFlexLayout }
                };
                messageBorder.Content = inBorderStackLayout;
            }
        }
        else
        {
            messageLabel = new Label
            {
                Text = message,
                TextColor = Color.FromArgb("#D1D5DB"),
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.WordWrap
            };
            messageBorder = new Border
            {
                Stroke = Color.FromArgb("#202127"),
                Margin = new Thickness(10, 0, 0, 0),
                StrokeThickness = 1,
                Padding = new Thickness(8, 5),
                BackgroundColor = Color.FromArgb("#444654"),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(5, 5, 5, 5)
                },
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Content = messageLabel
            };
            if (URLs.Length > 0)
            {
                var urlFlexLayout = new FlexLayout
                {
                    Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
                };
                foreach (var url in URLs)
                {
                    var urlLabel = new Label
                    {
                        Text = url,
                        TextColor = Color.FromArgb("#D1D5DB"),
                        FontSize = 16,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        LineBreakMode = LineBreakMode.WordWrap
                    };
                    var urlBorder = new Border
                    {
                        Stroke = Color.FromArgb("#202127"),
                        StrokeThickness = 1,
                        Padding = new Thickness(8, 5),
                        BackgroundColor = Color.FromArgb("#444654"),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = new CornerRadius(5, 5, 5, 5)
                        },
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Content = urlLabel
                    };
                    urlBorder.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () =>
                        {
                            await Clipboard.SetTextAsync(url);
                            await Launcher.OpenAsync(url);
                        })
                    });
                    urlFlexLayout.Children.Add(urlBorder);
                }
                var inBorderStackLayout = new StackLayout
                {
                    Children = { messageLabel, urlFlexLayout }
                };
                messageBorder.Content = inBorderStackLayout;
            }
        }
        await AppendSenderAndShowMessage(name, messageBorder, autoScroll);
    }

    public async Task AppendSenderAndShowMessage(string name, View content, bool autoScroll)
    {
        ChatStack.Dispatcher.Dispatch(() =>
        {
            if (name == CurrentName)
            {
                var messageGrid = new Grid
                {
                    Margin = new Thickness(20, 0, 20, 7),
                    Opacity = 0,
                    TranslationY = 50
                };
                messageGrid.Children.Add(content);
                if (lastSender == string.Empty || name != lastSender)
                {
                    lastSender = name;
                    content.Margin = new Thickness(0, 15, 0, 0);
                    messageGrid.RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    };
                    var senderLabel = new Label
                    {
                        Text = name,
                        TextColor = Color.FromArgb("#FFFFFF"),
                        FontSize = 18,
                        Padding = new Thickness(8, 8),
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };
                    var senderBorder = new Border
                    {
                        Stroke = Color.FromArgb("#202127"),
                        StrokeThickness = 2.5,
                        Margin = new Thickness(0, 18, 0, 0),
                        BackgroundColor = Color.FromArgb("#444654"),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = new CornerRadius(8, 8, 8, 8)
                        },
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Center,
                        Content = senderLabel
                    };
                    messageGrid.Children.Add(senderBorder);
                    Grid.SetRow(senderBorder, 0);
                    Grid.SetRow(messageGrid.Children[0] as BindableObject, 1);
                }
                ChatStack.Children.Add(messageGrid);
                messageGrid.TranslateTo(0, 0, 500, Easing.CubicOut);
                messageGrid.FadeTo(1, 500);
            }
            else
            {
                var messageGrid = new Grid
                {
                    BackgroundColor = Color.FromArgb("#444654"),
                    Padding = new Thickness(20, 0, 20, 7),
                    Opacity = 0,
                    TranslationY = 50
                };
                messageGrid.Children.Add(content);
                if (lastSender == string.Empty || name != lastSender)
                {
                    lastSender = name;
                    content.Margin = new Thickness(10, 5, 0, 0);
                    messageGrid.RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
                    };
                    var senderLabel = new Label
                    {
                        Text = name,
                        TextColor = Color.FromArgb("#FFFFFF"),
                        FontSize = 18,
                        Padding = new Thickness(8, 8),
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };
                    var senderBorder = new Border
                    {
                        Stroke = Color.FromArgb("#202127"),
                        StrokeThickness = 2.5,
                        Margin = new Thickness(0, 18, 0, 0),
                        BackgroundColor = Color.FromArgb("#343541"),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = new CornerRadius(8, 8, 8, 8)
                        },
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        Content = senderLabel
                    };
                    messageGrid.Children.Add(senderBorder);
                    Grid.SetRow(senderBorder, 0);
                    Grid.SetRow(messageGrid.Children[0] as BindableObject, 1);
                }
                ChatStack.Children.Add(messageGrid);
                messageGrid.TranslateTo(0, 0, 500, Easing.CubicOut);
                messageGrid.FadeTo(1, 500);
            }
        });
        if (autoScroll)
            await ChatScrollView.ScrollToAsync(0, ChatScrollView.ContentSize.Height, true);
    }
    #endregion

    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        if (CurrentName == string.Empty)
        {
            await DisplayAlert("用户名为空", "请先设置用户名", "确定");
        }
        else
        {
            if (connected)
            {
                try
                {
                    string message = InputEditor.Text;
                    InputEditor.Text = string.Empty;
                    var task = ShowTextMessage(CurrentName, message);
                    if (!string.IsNullOrWhiteSpace(message))
                        await xCCGroup.SendTextMessage(message);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("发送消息失败", ex.Message, "确定");
                }
            }
            else
            {
                await DisplayAlert("未连接到服务器", "请检查网络连接", "确定");
            }
        }
    }

    private void XCCNetWork_Connected(object sender, XCCConnectedEventArgs e)
    {
        Console.WriteLine($"连接到服务器：{e.XCCClientType}");
    }

    private async void XCCNetWork_ConnectionClosed(object sender, XCCConnectionClosedEventArgs e)
    {
        if (!e.ClosedNormally)
        {
            ShowConnectingTip();
            await xCCGroup.WaitConnect();
            if (!connected)
            {
                connected = true;
                HideConnectingTip();
            }
        }
    }

    private void InputEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputEditor.Text))
        {
            SendButton.BackgroundColor = Color.FromArgb("#A491E8");
            SendButton.IsEnabled = false;
        }
        else
        {
            SendButton.BackgroundColor = Color.FromArgb("#512BD4");
            SendButton.IsEnabled = true;
        }
    }

    private void ChatScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        isScrolled = true;
    }

    private bool SwitchToolBarButton(ImageButton sender)
    {
        if (lastButtonClicked == sender)
        {
            if (sender.ClassId.Contains("un"))
            {
                sender.Source = sender.ClassId.Replace("un", string.Empty);
                sender.ClassId = sender.ClassId.Replace("un", string.Empty);
                return true;
            }
            else
            {
                sender.Source = "un" + sender.ClassId;
                sender.ClassId = "un" + sender.ClassId;
                return false;
            }
        }
        else if (lastButtonClicked is not null)
        {
            lastButtonClicked.Source = "un" + lastButtonClicked.ClassId;
            lastButtonClicked.ClassId = "un" + lastButtonClicked.ClassId;
            sender.Source = sender.ClassId.Replace("un", string.Empty);
            lastButtonClicked = sender;
            return true;
        }
        else
        {
            sender.Source = sender.ClassId.Replace("un", string.Empty);
            sender.ClassId = sender.ClassId.Replace("un", string.Empty);
            lastButtonClicked = sender;
            return true;
        }
    }

    private void UnFocusAllButtonInToolBar()
    {
        foreach (var btn in ToolBarStackLayout.Children.Cast<ImageButton>().Where(btn => !btn.ClassId.Contains("un")))
        {
            btn.Source = "un" + btn.ClassId;
            btn.ClassId = "un" + btn.ClassId;
        }

        this.Dispatcher.Dispatch(() =>
        {
            ToolViewStackLayout.Children.Clear();
            ToolViewScrollView.IsVisible = false;
        });
    }

    private void ShowEmotionButton_Clicked(object sender, EventArgs e)
    {
        if (SwitchToolBarButton(sender as ImageButton))
        {
            this.Dispatcher.Dispatch(() =>
            {
                ToolViewScrollView.IsVisible = true;
                foreach (var btn in emotionImageButtonList)
                {
                    ToolViewStackLayout.Add(btn);
                }
                ToolViewScrollView.FadeTo(1, 200, Easing.CubicOut);
            });
            InputEditor.Unfocus();
        }
        else
        {
            ToolViewScrollView.FadeTo(0, 200, Easing.SpringOut);
            ToolViewScrollView.IsVisible = false;
            ToolViewStackLayout.Children.Clear();
        }
    }

    private void InputEditor_Focused(object sender, FocusEventArgs e)
    {
        ToolViewScrollView.FadeTo(0, 200, Easing.SpringOut);
        ToolViewScrollView.IsVisible = false;
        ToolViewStackLayout.Children.Clear();
        UnFocusAllButtonInToolBar();
    }

    private async void ShowImageButton_Clicked(object sender, EventArgs e)
    {
        PermissionStatus m_statusRequestStorageRead = await Permissions.RequestAsync<Permissions.Photos>();
        switch (m_statusRequestStorageRead)
        {
            case PermissionStatus.Granted:
                try
                {
                    var fileResult = await MediaPicker.PickPhotoAsync();
                    if (fileResult is not null)
                        SendSelectedImage(fileResult);
                }
                catch (Exception ex)
                {
                    await PopupAction.DisplayPopup(new ErrorPopup("读取照片时出现错误", ex.Message));
                }
                break;
            case PermissionStatus.Denied:
                await DisplayAlert("请求拒绝", "无法读取图片，请前往手机设置打开读写", "确认");
                break;
            case PermissionStatus.Disabled:
                await DisplayAlert("暂无权限", "读取图片的请求被拒绝，请前往权限管理打开权限", "确认");
                break;
            case PermissionStatus.Unknown:
                await DisplayAlert("我 氵则", "请求处于未知状态？？？？？？？？？", "啊这（截图和我反馈）");
                break;
            default:
                await DisplayAlert("这位更是个寄吧", "出现错误", "我测");
                break;
        }
    }

    private async void CameraButton_Clicked(object sender, EventArgs e)
    {
        PermissionStatus m_statusRequestStorageRead = await Permissions.RequestAsync<Permissions.Photos>();
        switch (m_statusRequestStorageRead)
        {
            case PermissionStatus.Granted:
                try
                {
                    var fileResult = await MediaPicker.CapturePhotoAsync();
                    if (fileResult is not null)
                        SendSelectedImage(fileResult);
                }
                catch (Exception ex)
                {
                    await PopupAction.DisplayPopup(new ErrorPopup("打开摄像头时出现错误", ex.Message));
                }
                break;
            case PermissionStatus.Denied:
                await DisplayAlert("请求拒绝", "无法拍摄图片，请前往手机设置打开摄像头权限", "确认");
                break;
            case PermissionStatus.Disabled:
                await DisplayAlert("暂无权限", "获取摄像头权限被拒绝", "确认");
                break;
            case PermissionStatus.Unknown:
                await DisplayAlert("我 氵则", "请求处于未知状态？？？？？？？？？", "啊这（截图和我反馈）");
                break;
            default:
                await DisplayAlert("这位更是个寄吧", "出现错误", "我测");
                break;
        }
    }
}