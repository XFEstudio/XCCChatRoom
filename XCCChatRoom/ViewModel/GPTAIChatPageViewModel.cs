using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiPopup;
using Microsoft.Maui.Controls.Shapes;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XCCChatRoom.Profiles;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.ArrayExtension;
using XFEExtension.NetCore.TaskExtension;
using XFEExtension.NetCore.XFEChatGPT;
using XFEExtension.NetCore.XFEChatGPT.ChatGPTInnerClass.HelperClass;

namespace XCCChatRoom.ViewModel;

internal partial class GPTAIChatPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string askText;

    public GPTAIChatPage ViewPage { get; init; }
    public static string CurrentDialogId { get; private set; }

    private string dialogTitle = string.Empty;
    public string DialogTitle
    {
        get
        {
            return dialogTitle;
        }
        set
        {
            dialogTitle = value;
            var result = GPTAIDialogManager.GetDialog(CurrentDialogId);
            if (result is not null)
            {
                result["Title"] = value;
                GPTAIDialogManager.XFEEntries[CurrentDialogId] = result.ToString();
            }
            ViewPage.Title = value.Length > 10 ? value[..10] + "..." : value;
        }
    }

    private string gptVersion = "GPT-3.5";
    public string GPTVersion
    {
        get
        {
            return gptVersion;
        }
        set
        {
            gptVersion = value;
            var result = GPTAIDialogManager.GetDialog(CurrentDialogId);
            if (result is not null)
            {
                result["GPTVersion"] = value;
                GPTAIDialogManager.XFEEntries[CurrentDialogId] = result.ToString();
            }
        }
    }

    private string system = "你是一个由寰宇朽力网络科技开发的对接ChatGPT的人工智能AI";
    public string System
    {
        get
        {
            return system;
        }
        set
        {
            system = value;
            var result = GPTAIDialogManager.GetDialog(CurrentDialogId);
            if (result is not null)
            {
                result["System"] = value;
                GPTAIDialogManager.XFEEntries[CurrentDialogId] = result.ToString();
            }
        }
    }

    private static readonly MemorableXFEChatGPT memorableXFEChatGPT = GPTAIDialogManager.MemorableXFEChatGPT;
    private static bool isInitialized = false;
    private static readonly List<LabelAndMessageId> labelAndMessageIdList = [];
    private string lastQuestion = string.Empty;
    private bool lastAnswerGenerated = true;
    private bool autoScrollEnable = true;
    private double lastScrollDistance = 0;

    public GPTAIChatPageViewModel(GPTAIChatPage viewPage)
    {
        ViewPage = viewPage;
        if (!isInitialized)
        {
            isInitialized = true;
            CurrentDialogId = Guid.NewGuid().ToString();
            GPTAIDialogManager.SuggestionsReceived += GPTAIDialogManager_SuggestionsReceived;
            GPTAIDialogManager.GetSuggestionFromGPT();
            memorableXFEChatGPT.XFEChatGPTMessageReceived += ReceiveGPTResponse;
            memorableXFEChatGPT.CreateDialog(CurrentDialogId, system, true, true);
        }
        else
        {
            if (GPTAIDialogManager.GetDialog(CurrentDialogId) is null && GPTAIDialogManager.Suggestion is not null)
            {
                foreach (var suggest in GPTAIDialogManager.Suggestion.Split("|", StringSplitOptions.RemoveEmptyEntries))
                {
                    var suggestionLabel = new Label
                    {
                        Text = suggest,
                        TextColor = Color.Parse("White"),
                        FontSize = 18,
                        LineBreakMode = LineBreakMode.WordWrap,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    var suggestionBorder = new Border
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Stroke = Color.FromArgb("#1b1c22"),
                        StrokeThickness = 2,
                        Margin = new Thickness(5),
                        Padding = new Thickness(15, 5),
                        BackgroundColor = Color.FromArgb("#444654"),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 10
                        },
                        MaximumWidthRequest = 150,
                        Content = suggestionLabel,
                        GestureRecognizers =
                            {
                                new TapGestureRecognizer
                                {
                                    CommandParameter = suggestionLabel,
                                    Command = new Command(sender =>
                                    {
                                        ViewPage.inputEditor.Text = (sender as Label).Text;
                                        _ = SendButtonClick();
                                    })
                                }
                            }
                    };
                    ViewPage.Dispatcher.Dispatch(() =>
                    {
                        ViewPage.suggestionFlexLayout.Add(suggestionBorder);
                    });
                }
            }
            else
            {
                LoadDialog(CurrentDialogId);
            }
        }
    }

    #region 事件
    private void GPTAIDialogManager_SuggestionsReceived(object sender, string e)
    {
        if (e.Contains('|'))
        {
            var result = e.Split("|");
            var suggestionLabel = new Label
            {
                Text = result[1],
                TextColor = Color.Parse("White"),
                FontSize = 18,
                LineBreakMode = LineBreakMode.WordWrap,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            var suggestionBorder = new Border
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Stroke = Color.FromArgb("#1b1c22"),
                StrokeThickness = 2,
                Margin = new Thickness(5),
                Padding = new Thickness(15, 5),
                BackgroundColor = Color.FromArgb("#444654"),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 10
                },
                MaximumWidthRequest = 150,
                Content = suggestionLabel,
                GestureRecognizers =
                {
                    new TapGestureRecognizer
                    {
                        CommandParameter = suggestionLabel,
                        Command = new Command(sender =>
                        {
                            AskText = (sender as Label).Text;
                            _ = SendButtonClick();
                        })
                    }
                }
            };
            if (ViewPage.suggestionFlexLayout.Count > 0 && ViewPage.suggestionFlexLayout.Last() is Border border)
            {
                if (border.Content is Label label)
                {
                    label.Dispatcher.Dispatch(() =>
                    {
                        label.Text += result[0];
                    });
                }
            }
            ViewPage.Dispatcher.Dispatch(() =>
            {
                ViewPage.suggestionFlexLayout.Add(suggestionBorder);
            });
        }
        else
        {
            if (ViewPage.suggestionFlexLayout.Last() is Border border)
            {
                if (border.Content is Label label)
                {
                    label.Dispatcher.Dispatch(() =>
                    {
                        label.Text += e;
                    });
                }
            }
        }
    }

    private async void CopyButton_Clicked(object sender, EventArgs e)
    {
        await Clipboard.SetTextAsync((((sender as ImageButton).Parent as Grid).Children[0] as Label).Text);
        await PopupAction.DisplayPopup(new TipPopup("复制成功", 2));
    }

    private void ReceiveGPTResponse(object sender, XFEGPTMessageReceivedEventArgs e)
    {
        var labelAndMessage = labelAndMessageIdList.Find(x => x.messageId == e.MessageId);
        if (labelAndMessage is not null)
        {
            switch (e.GenerateState)
            {
                case GenerateState.Start:
                    break;
                case GenerateState.Continue:
                    if (labelAndMessageIdList.Count > 0)
                    {
                        if (!labelAndMessage.completeState)
                        {
                            ViewPage.chatStack.Dispatcher.Dispatch(() =>
                            {
                                var label = labelAndMessage.label;
                                label.Text = label.Text.Insert(label.Text.Length - 1, e.Message);
                            });
                            ViewPage.chatScrollView.Dispatcher.Dispatch(() =>
                            {
                                if (autoScrollEnable)
                                {
                                    ViewPage.chatScrollView.ScrollToAsync(ViewPage.chatStack, ScrollToPosition.End, false);
                                }
                            });
                            var result = GPTAIDialogManager.GetDialog(CurrentDialogId);
                            result["Content"] = memorableXFEChatGPT.GetDialog(CurrentDialogId)[1..].ToXFEString("Content");
                            GPTAIDialogManager.XFEEntries[CurrentDialogId] = result.ToString();
                            GPTAIDialogManager.SaveDialogs();
                        }
                    }
                    break;
                case GenerateState.Error:
                    if (labelAndMessageIdList.Count > 0)
                    {
                        ViewPage.chatStack.Dispatcher.Dispatch(() =>
                        {
                            labelAndMessage.completeState = true;
                            labelAndMessage.label.TextColor = Color.FromArgb("#F87171");
                            labelAndMessage.label.Text = labelAndMessage.label.Text.Insert(labelAndMessage.label.Text.Length - 1, $"发生错误：\n{e.Message}");
                            Console.Write(e.Message);
                        });
                    }
                    break;
                case GenerateState.End:
                    GPTAIDialogManager.SaveDialogs();
                    labelAndMessage.completeState = true;
                    break;
                default:
                    ProcessException.ShowEnumException();
                    break;
            }
        }
    }

    public void ChatScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        if (lastScrollDistance > e.ScrollY)
            autoScrollEnable = false;
        else
            lastScrollDistance = e.ScrollY;
    }

    public void InputEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(AskText) || !lastAnswerGenerated)
        {
            ViewPage.sendButton.BackgroundColor = Color.FromArgb("#A491E8");
            ViewPage.sendButton.IsEnabled = false;
        }
        else
        {
            ViewPage.sendButton.BackgroundColor = Color.FromArgb("#512BD4");
            ViewPage.sendButton.IsEnabled = true;
        }
    }
    #endregion

    #region 逻辑
    private void TextMouseUpdate(LabelAndMessageId labelAndMessageId)
    {
        bool textMouseState = false;
        bool hasTextMouse = false;
        //实现光标闪烁
        while (true)
        {
            if (!labelAndMessageId.completeState)
            {
                ViewPage.chatStack.Dispatcher.Dispatch(() =>
                {
                    if (textMouseState)
                    {
                        //替换最后一个字符为空
                        labelAndMessageId.label.Text = labelAndMessageId.label.Text.Remove(labelAndMessageId.label.Text.Length - 1) + " ";
                        textMouseState = false;
                    }
                    else
                    {
                        if (labelAndMessageId.label.Text.Length > 0)
                        {
                            //替换最后一个字符为指定字符
                            labelAndMessageId.label.Text = labelAndMessageId.label.Text.Remove(labelAndMessageId.label.Text.Length - 1) + "▌";
                        }
                        else
                        {
                            labelAndMessageId.label.Text = "▌";
                        }
                        textMouseState = true;
                        hasTextMouse = true;
                    }
                });
            }
            else
            {
                ViewPage.Dispatcher.Dispatch(() =>
                {
                    lastAnswerGenerated = true;
                    ViewPage.sendButton.IsEnabled = true;
                    ViewPage.regenerateBorder.IsVisible = true;
                });
                break;
            }
            Thread.Sleep(500);
        }
        if (hasTextMouse)
        {
            ViewPage.chatStack.Dispatcher.Dispatch(() =>
            {
                //移除最后一个字符
                labelAndMessageId.label.Text = labelAndMessageId.label.Text.Remove(labelAndMessageId.label.Text.Length - 1);
            });
        }
    }

    public void CreateNewDialog() => LoadDialog(Guid.NewGuid().ToString());

    public void LoadDialog(string dialogId)
    {
        ViewPage.chatStack.Clear();
        lastQuestion = string.Empty;
        lastAnswerGenerated = true;
        AskText = string.Empty;
        CurrentDialogId = dialogId;
        GPTAIDialogManager.SuggestionsReceived -= GPTAIDialogManager_SuggestionsReceived;
        ViewPage.suggestionFlexLayout.Clear();
        if (GPTAIDialogManager.GetDialog(CurrentDialogId) is null)
        {
            GPTAIDialogManager.SuggestionsReceived += GPTAIDialogManager_SuggestionsReceived;
            ViewPage.suggestionFlexLayout.IsVisible = true;
            GPTAIDialogManager.GetSuggestionFromGPT();
            return;
        }
        ViewPage.suggestionFlexLayout.IsVisible = false;
        DialogTitle = GPTAIDialogManager.GetDialog(CurrentDialogId)["Title"];
        GPTVersion = GPTAIDialogManager.GetDialog(CurrentDialogId)["GPTVersion"];
        System = GPTAIDialogManager.GetDialog(CurrentDialogId)["System"];
        var result = GPTAIDialogManager.GetDialog(CurrentDialogId);
        var chatDialog = (result["Content"] ??= string.Empty).ToXFEArray<string>();
        for (int i = 0; i < chatDialog.Length; i += 2)
        {
            var askLabel = new Label
            {
                Text = chatDialog[i],
                TextColor = Color.FromArgb("#ECECF1"),
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.WordWrap
            };
            var askGrid = new Grid
            {
                Margin = new Thickness(20, 20, 20, 20),
                Children = { askLabel }
            };
            ViewPage.chatStack.Children.Add(askGrid);
            lastQuestion = chatDialog[i];
            var responseLabel = new Label
            {
                Text = chatDialog.Length > i + 1 ? chatDialog[i + 1] : string.Empty,
                TextColor = Color.FromArgb("#D1D5DB"),
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.WordWrap
            };
            var responseGrid = new Grid
            {
                BackgroundColor = Color.FromArgb("#444654"),
                Padding = new Thickness(20, 20, 20, 20),
                Children = { responseLabel }
            };
            ViewPage.chatStack.Children.Add(responseGrid);
            labelAndMessageIdList.Add(new LabelAndMessageId(responseLabel, Guid.NewGuid().ToString()));
        }
        ViewPage.chatScrollView.Dispatcher.Dispatch(() =>
        {
            ViewPage.chatScrollView.ScrollToAsync(ViewPage.chatStack, ScrollToPosition.End, false);
        });
        ViewPage.regenerateBorder.IsVisible = true;
    }
    #endregion

    #region Command
    [RelayCommand]
    static async Task EditSetting() => await PopupAction.DisplayPopup(new GPTSettingPopup());

    [RelayCommand]
    static async Task ShowHistory() => await PopupAction.DisplayPopup(new GPTDialogListPopup(CurrentDialogId));

    [RelayCommand]
    async Task SendButtonClick()
    {
        if (GPTAIDialogManager.GetDialog(CurrentDialogId) is null)
        {
            DialogTitle = AskText;
            GPTAIDialogManager.AddDialog(CurrentDialogId, DialogTitle, System, "GPT-3.5", [AskText]);
            GPTAIDialogManager.SuggestionsReceived -= GPTAIDialogManager_SuggestionsReceived;
            ViewPage.suggestionFlexLayout.IsVisible = false;
        }
        UserInfoProfile.CurrentUser.NormalGPTUsage++;
        UserInfoProfile.CurrentUser.TotalNormalGPTUsage++;
        await UserInfoPage.UpLoadUserInfo();
        autoScrollEnable = true;
        lastQuestion = AskText;
        lastAnswerGenerated = false;
        ViewPage.sendButton.IsEnabled = false;
        ViewPage.regenerateBorder.IsVisible = false;
        ViewPage.stopGenerateBorder.IsVisible = true;
        ViewPage.chatStack.Dispatcher.Dispatch(async () =>
        {
            var askCopyButton = new ImageButton
            {
                Source = "copy",
                HeightRequest = 20,
                WidthRequest = 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
            };
            askCopyButton.Clicked += CopyButton_Clicked;
            var askLabel = new Label
            {
                Text = AskText,
                TextColor = Color.FromArgb("#ECECF1"),
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.WordWrap
            };
            var askGrid = new Grid
            {
                Margin = new Thickness(20, 20, 20, 20),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width = new GridLength(30,GridUnitType.Absolute)
                    },
                    new ColumnDefinition
                    {
                        Width = GridLength.Star
                    },
                    new ColumnDefinition
                    {
                        Width = new GridLength(30,GridUnitType.Absolute)
                    }
                }
            };
            askGrid.Add(askLabel, 1);
            askGrid.Add(askCopyButton, 2);
            ViewPage.chatStack.Add(askGrid);
            var responseCopyButton = new ImageButton
            {
                Source = "copy",
                HeightRequest = 20,
                WidthRequest = 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
            };
            responseCopyButton.Clicked += CopyButton_Clicked;
            var responseLabel = new Label
            {
                Text = string.Empty,
                TextColor = Color.FromArgb("#D1D5DB"),
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.WordWrap
            };
            var responseGrid = new Grid
            {
                BackgroundColor = Color.FromArgb("#444654"),
                Padding = new Thickness(20, 20, 20, 20),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width = new GridLength(30,GridUnitType.Absolute)
                    },
                    new ColumnDefinition
                    {
                        Width = GridLength.Star
                    },
                    new ColumnDefinition
                    {
                        Width = new GridLength(30,GridUnitType.Absolute)
                    }
                }
            };
            responseGrid.Add(responseLabel, 1);
            responseGrid.Add(responseCopyButton, 2);
            ViewPage.chatStack.Add(responseGrid);
            Guid guid = Guid.NewGuid();
            var labelAndString = new LabelAndMessageId(responseLabel, guid.ToString());
            labelAndMessageIdList.Add(labelAndString);
            var task = new Action(() => TextMouseUpdate(labelAndString)).StartNewTask();
            await ViewPage.chatScrollView.ScrollToAsync(0, ViewPage.chatScrollView.ContentSize.Height + 500, false);
            try
            {
                memorableXFEChatGPT.AskChatGPT(CurrentDialogId, guid.ToString(), AskText);
            }
            catch (Exception ex)
            {
                await PopupAction.DisplayPopup(new ErrorPopup("发生错误", ex.Message));
            }
            AskText = string.Empty;
        });
    }

    [RelayCommand]
    void Regenerate()
    {
        if (ViewPage.chatStack.Count > 0)
        {
            var lastMessage = labelAndMessageIdList.Last();
            lastMessage.label.Text = string.Empty;
            lastMessage.label.TextColor = Color.FromArgb("#D1D5DB");
            lastMessage.messageId = Guid.NewGuid().ToString();
            lastMessage.completeState = false;
            ViewPage.sendButton.IsEnabled = false;
            ViewPage.regenerateBorder.IsVisible = false;
            ViewPage.stopGenerateBorder.IsVisible = true;
            lastAnswerGenerated = false;
            var currentDialogCollection = memorableXFEChatGPT.MemoryDialog[CurrentDialogId];
            if (currentDialogCollection.Count > 1)
                currentDialogCollection.RemoveAt(currentDialogCollection.Count - 1);
            if (currentDialogCollection.Count > 1)
                currentDialogCollection.RemoveAt(currentDialogCollection.Count - 1);
            new Action(() => TextMouseUpdate(lastMessage)).StartNewTask();
            memorableXFEChatGPT.AskChatGPT(CurrentDialogId, lastMessage.messageId, lastQuestion);
        }
    }

    [RelayCommand]
    void StopGenerate()
    {
        ViewPage.stopGenerateBorder.IsVisible = false;
        ViewPage.regenerateBorder.IsVisible = true;
        labelAndMessageIdList.Last().completeState = true;
        lastAnswerGenerated = true;
        ViewPage.sendButton.IsEnabled = true;
    }
    #endregion
}
