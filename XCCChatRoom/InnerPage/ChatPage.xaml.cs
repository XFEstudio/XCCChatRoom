using Microsoft.Maui.Controls.Shapes;
#if ANDROID
using Android.Media;
#endif
using XFE各类拓展.CyberComm.XCCNetWork;
using XFE各类拓展.TaskExtension;
using Image = Microsoft.Maui.Controls.Image;
using XFE各类拓展.FormatExtension;
using Plugin.LocalNotification;
using XCCChatRoom.AllImpl;
using XFE各类拓展.StringExtension;

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
            StartGroupComm(value);
        }
    }
    public static ChatPage CurrentInstance { get; private set; }
    public string CurrentName { get; set; }
    #endregion
    #region 字段
    private List<ImageButton> emotionImageButtonList = new List<ImageButton>();
    private List<Image> emotionImageList = new List<Image>();
    private XCCNetWork xCCNetWork;
    private XCCGroup xCCGroup;
    private Grid loadGrid;
    private Image serverImg;
    private ImageButton lastButtonClicked = null;
    private ToolbarItem phoneCallItem;
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
        xCCNetWork.Connected += XCCNetWork_Connected;
        xCCNetWork.ConnectionClosed += XCCNetWork_ConnectionClosed;
        xCCNetWork.MessageReceived += XCCGroup_MessageReceived;
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

    private async void EmotionImageButtonButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            await xCCGroup.SendStandardTextMessage(CurrentName, $"[Emotion]{(sender as ImageButton).ClassId}");
            await ShowMessage(CurrentName, string.Empty, (sender as ImageButton).ClassId);
        }
        catch (Exception ex)
        {
            await DisplayAlert("发送消息失败", ex.Message, "确定");
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
    public async void StartGroupComm(string name)
    {
        xCCGroup = xCCNetWork.CreateGroup(name);
        try
        {
            xCCGroup.StartXCC(true, 50);
        }
        catch (Exception ex)
        {
            await DisplayAlert("网络错误", ex.Message, "确定");
        }
        ShowConnectingTip();
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
    public async Task ShowMessage(string name, string message, string image = null, bool autoScroll = true)
    {
        ChatStack.Dispatcher.Dispatch(() =>
        {
            if (name == CurrentName)
            {
                var URLs = message.GetUrl();
                var messageLabel = new Label
                {
                    Text = message,
                    TextColor = Color.FromArgb("#ECECF1"),
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    LineBreakMode = LineBreakMode.WordWrap
                };
                var messageBorder = new Border
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
                var messageGrid = new Grid
                {
                    Margin = new Thickness(20, 0, 20, 7),
                    Opacity = 0,
                    TranslationY = 50
                };
                if (image is not null)
                {
                    var imageView = new Image
                    {
                        Source = image,
                        WidthRequest = 100,
                        HeightRequest = 100,
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Center,
                        ClassId = image
                    };
                    messageGrid.Children.Add(imageView);
                }
                else
                {
                    messageGrid.Children.Add(messageBorder);
                }
                if (lastSender == string.Empty || name != lastSender)
                {
                    lastSender = name;
                    messageBorder.Margin = new Thickness(0, 15, 0, 0);
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
                var URLs = message.GetUrl();
                var messageLabel = new Label
                {
                    Text = message,
                    TextColor = Color.FromArgb("#D1D5DB"),
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    LineBreakMode = LineBreakMode.WordWrap
                };
                var messageBorder = new Border
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
                var messageGrid = new Grid
                {
                    BackgroundColor = Color.FromArgb("#444654"),
                    Padding = new Thickness(20, 0, 20, 7),
                    Opacity = 0,
                    TranslationY = 50
                };
                if (image is not null)
                {
                    var imageView = new Image
                    {
                        Source = image,
                        WidthRequest = 100,
                        HeightRequest = 100,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Center,
                        ClassId = image
                    };
                    messageGrid.Children.Add(imageView);
                }
                else
                {
                    messageGrid.Children.Add(messageBorder);
                }
                if (lastSender == string.Empty || name != lastSender)
                {
                    lastSender = name;
                    messageBorder.Margin = new Thickness(10, 5, 0, 0);
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
                    var task = ShowMessage(CurrentName, message);
                    if (!string.IsNullOrWhiteSpace(message))
                        await xCCGroup.SendStandardTextMessage(CurrentName, message);
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
        if (firstConnect)
        {
            try
            {
                xCCGroup.GetHistory();
                firstConnect = false;
                HideConnectingTip();
            }
            catch (Exception ex)
            {
                DisplayAlert("获取历史消息失败", ex.Message, "确定");
            }
        }
        connected = true;
    }
    private void XCCNetWork_ConnectionClosed(object sender, XCCConnectionClosedEventArgs e)
    {
        if (!e.ClosedNormally)
        {
            ShowConnectingTip();
        }
    }
    private async void XCCGroup_MessageReceived(object sender, XCCMessageReceivedEventArgs e)
    {
        switch (e.MessageType)
        {
            case XCCMessageType.Text:
                string message = e.TextMessage;
                bool isHistory = false;
                if (e.TextMessage.Contains("[XCCGetHistory]"))
                {
                    message = e.TextMessage.Replace("[XCCGetHistory]", string.Empty);
                    isHistory = true;
                }
                var dictionary = new XFEMultiDictionary(message);
                if (dictionary.Count > 0)
                {
                    foreach (var entry in dictionary)
                    {
                        if (isHistory)
                        {
                            if (entry.Content.Contains("[Emotion]"))
                            {
                                var image = entry.Content.Replace("[Emotion]", string.Empty);
                                await ShowMessage(entry.Header, entry.Content, image, false);
                            }
                            else
                            {
                                await ShowMessage(entry.Header, entry.Content, null, false);
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
                                Description = $"{entry.Header}：{entry.Content}",
                                BadgeNumber = MessageCount++,
                                Image = iconImg,

                            };
                            await LocalNotificationCenter.Current.Show(request);
#endif
                            if (e.TextMessage.Contains("[Emotion]"))
                            {
                                var image = entry.Content.Replace("[Emotion]", string.Empty);
                                await ShowMessage(entry.Header, entry.Content, image);
                            }
                            else
                            {
                                await ShowMessage(entry.Header, entry.Content);
                            }
                        }
                    }
                }
                else
                {
                    ChatStack.Dispatcher.Dispatch(() =>
                    {
                        var messageLabel = new Label
                        {
                            Text = message,
                            TextColor = Color.FromArgb("#D1D5DB"),
                            FontSize = 18,
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.Center,
                            LineBreakMode = LineBreakMode.WordWrap

                        };
                        var messageGrid = new Grid
                        {
                            BackgroundColor = Color.FromArgb("#444654"),
                            Padding = new Thickness(20, 20, 20, 20),
                            Children = { messageLabel }
                        };
                        ChatStack.Children.Add(messageGrid);
                    });
                }
                if (isHistory)
                {
                    ChatStack.Dispatcher.Dispatch(() =>
                    {
                        var messageLabel = new Label
                        {
                            Text = "以上为历史消息",
                            Opacity = 0.3,
                            TextColor = Color.FromArgb("#D1D5DB"),
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontSize = 18,
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.Center,
                            LineBreakMode = LineBreakMode.WordWrap
                        };
                        if (dictionary.Count == 0)
                        {
                            messageLabel.Text = "暂无历史消息";
                        }
                        var messageGrid = new Grid
                        {
                            BackgroundColor = Color.FromArgb("#444654"),
                            Padding = new Thickness(20, 0, 20, 0),
                            Margin = new Thickness(0, 5),
                            Children = { messageLabel }
                        };
                        ChatStack.Children.Add(messageGrid);
                        new Action(async () =>
                        {
                            while (!isScrolled)
                            {
                                try
                                {
                                    await ChatScrollView.ScrollToAsync(0, ChatStack.DesiredSize.Height, false);
                                }
                                catch { }
                            }
                        }).StartNewTask();
                    });
                }
                break;

            case XCCMessageType.Binary:
                if (phoneCallEnabled)
                {
#if ANDROID
                    PlayAudioData(e.BinaryMessage);
#endif
                }
                break;
            case XCCMessageType.Error:
                connected = false;
                ChatStack.Dispatcher.Dispatch(() =>
                {
                    ChatStack.Dispatcher.Dispatch(() =>
                    {
                        var messageLabel = new Label
                        {
                            Text = $"发生错误：{e.Exception.Message}",
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
                    Console.WriteLine(e.Exception);
                });
                await DisplayAlert("网路错误", e.Exception.Message, "确定");
                break;
            default:
                ProcessException.ShowEnumException();
                break;
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
        foreach (ImageButton btn in ToolBarStackLayout.Children)
        {
            if (!btn.ClassId.Contains("un"))
            {
                btn.Source = "un" + btn.ClassId;
                btn.ClassId = "un" + btn.ClassId;
            }
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
}