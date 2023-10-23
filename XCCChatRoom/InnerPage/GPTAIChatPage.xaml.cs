using MauiPopup;
using Microsoft.Maui.Controls.Shapes;
using XCCChatRoom.AllImpl;
using XCCChatRoom.Controls;
using XFE各类拓展.ArrayExtension;
using XFE各类拓展.TaskExtension;
using XFE各类拓展.XFEChatGPT;

namespace XCCChatRoom.InnerPage;

public partial class GPTAIChatPage : ContentPage
{
    public static GPTAIChatPage Current { get; private set; }
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
            Title = value.Length > 10 ? value[..10] + "..." : value;
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
    private static MemorableXFEChatGPT memorableXFEChatGPT = GPTAIDialogManager.MemorableXFEChatGPT;
    private static bool isInitialized = false;
    private static List<LabelAndMessageId> labelAndMessageIdList = new List<LabelAndMessageId>();
    private string lastQuestion = string.Empty;
    private bool lastAnswerGenerated = true;
    private bool autoScrollEnable = true;
    private bool autoScrolled = false;
    public GPTAIChatPage()
    {
        Current = this;
        InitializeComponent();
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
                                        InputEditor.Text = (sender as Label).Text;
                                        SendButton_Clicked(null, null);
                                    })
                                }
                            }
                    };
                    this.Dispatcher.Dispatch(() =>
                    {
                        SuggestionFlexLayout.Add(suggestionBorder);
                    });
                }
            }
            else
            {
                LoadDialog(CurrentDialogId);
            }
        }
        backButton.Command = new Command(() =>
        {
            GPTAIDialogManager.SaveDialogs();
            Navigation.PopAsync();
        });
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "编辑设定",
            IconImageSource = "edit",
            Command = new Command(async () =>
            {
                await PopupAction.DisplayPopup(new GPTSettingPopup());
            })
        });
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "历史对话",
            IconImageSource = "history",
            Command = new Command(async () =>
            {
                await PopupAction.DisplayPopup(new GPTDialogListPopup(CurrentDialogId));
            })
        });
    }

    protected override bool OnBackButtonPressed()
    {
        GPTAIDialogManager.SaveDialogs();
        return base.OnBackButtonPressed();
    }

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
                            InputEditor.Text = (sender as Label).Text;
                            SendButton_Clicked(null, null);
                        })
                    }
                }
            };
            if (SuggestionFlexLayout.Count > 0 && SuggestionFlexLayout.Last() is Border border)
            {
                if (border.Content is Label label)
                {
                    label.Dispatcher.Dispatch(() =>
                    {
                        label.Text += result[0];
                    });
                }
            }
            this.Dispatcher.Dispatch(() =>
            {
                SuggestionFlexLayout.Add(suggestionBorder);
            });
        }
        else
        {
            if (SuggestionFlexLayout.Last() is Border border)
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

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if ANDROID
        (InputEditor.Handler.PlatformView as Android.Widget.EditText).Background = null;
#endif
    }

    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        if (GPTAIDialogManager.GetDialog(CurrentDialogId) is null)
        {
            DialogTitle = InputEditor.Text;
            GPTAIDialogManager.AddDialog(CurrentDialogId, DialogTitle, System, "GPT-3.5", new string[] { InputEditor.Text });
            GPTAIDialogManager.SuggestionsReceived -= GPTAIDialogManager_SuggestionsReceived;
            SuggestionFlexLayout.IsVisible = false;
        }
        UserInfo.CurrentUser.NormalGPTUsage++;
        UserInfo.CurrentUser.TotalNormalGPTUsage++;
        await UserInfo.UpLoadUserInfo();
        autoScrollEnable = true;
        autoScrolled = true;
        await ChatScrollView.ScrollToAsync(ChatStack, ScrollToPosition.End, false);
        lastQuestion = InputEditor.Text;
        lastAnswerGenerated = false;
        SendButton.IsEnabled = false;
        RegenerateBorder.IsVisible = false;
        StopGenerateBorder.IsVisible = true;
        ChatStack.Dispatcher.Dispatch(async () =>
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
                Text = InputEditor.Text,
                TextColor = Color.FromArgb("#ECECF1"),
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.WordWrap
            };
            var askGrid = new Grid
            {
                Margin = new Thickness(20, 20, 20, 20),
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width = GridLength.Star
                    },
                    new ColumnDefinition
                    {
                        Width = new GridLength(10, GridUnitType.Star)
                    },
                    new ColumnDefinition
                    {
                        Width = GridLength.Star
                    }
                }
            };
            askGrid.Add(askLabel, 1);
            askGrid.Add(askCopyButton, 2);
            ChatStack.Add(askGrid);
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
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width = GridLength.Star
                    },
                    new ColumnDefinition
                    {
                        Width = new GridLength(10, GridUnitType.Star)
                    },
                    new ColumnDefinition
                    {
                        Width = GridLength.Star
                    }
                }
            };
            responseGrid.Add(responseLabel, 1);
            responseGrid.Add(responseCopyButton, 2);
            ChatStack.Add(responseGrid);
            Guid guid = Guid.NewGuid();
            var labelAndString = new LabelAndMessageId(responseLabel, guid.ToString());
            labelAndMessageIdList.Add(labelAndString);
            var task = new Action(() => TextMouseUpdate(labelAndString)).StartNewTask();
            try
            {
                memorableXFEChatGPT.AskChatGPT(CurrentDialogId, guid.ToString(), InputEditor.Text);
            }
            catch (Exception ex)
            {
                await PopupAction.DisplayPopup(new ErrorPopup("发生错误", ex.Message));
            }
            InputEditor.Text = string.Empty;
        });
    }

    private async void CopyButton_Clicked(object sender, EventArgs e)
    {
        await Clipboard.SetTextAsync((((sender as ImageButton).Parent as Grid).Children[0] as Label).Text);
        await PopupAction.DisplayPopup(new LoadingPopup("复制成功", 2));
    }

    private void ReceiveGPTResponse(object sender, XFEGPTMessageReceivedEventArgs e)
    {
        var labelAndMessage = labelAndMessageIdList.Find(x => x.messageId == e.messageId);
        if (labelAndMessage is not null)
        {
            switch (e.generateState)
            {
                case GenerateState.Start:
                    break;
                case GenerateState.Continue:
                    if (labelAndMessageIdList.Count > 0)
                    {
                        if (!labelAndMessage.completeState)
                        {
                            ChatStack.Dispatcher.Dispatch(() =>
                            {
                                var label = labelAndMessage.label;
                                label.Text = label.Text.Insert(label.Text.Length - 1, e.message);
                            });
                            ChatScrollView.Dispatcher.Dispatch(() =>
                            {
                                if (autoScrollEnable)
                                {
                                    autoScrolled = true;
                                    ChatScrollView.ScrollToAsync(ChatStack, ScrollToPosition.End, false);
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
                        ChatStack.Dispatcher.Dispatch(() =>
                        {
                            labelAndMessage.completeState = true;
                            labelAndMessage.label.TextColor = Color.FromArgb("#F87171");
                            labelAndMessage.label.Text = labelAndMessage.label.Text.Insert(labelAndMessage.label.Text.Length - 1, $"发生错误：\n{e.message}");
                            Console.Write(e.message);
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

    private void TextMouseUpdate(LabelAndMessageId labelAndMessageId)
    {
        bool textMouseState = false;
        bool hasTextMouse = false;
        //实现光标闪烁
        while (true)
        {
            if (!labelAndMessageId.completeState)
            {
                ChatStack.Dispatcher.Dispatch(() =>
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
                this.Dispatcher.Dispatch(() =>
                {
                    lastAnswerGenerated = true;
                    SendButton.IsEnabled = true;
                    RegenerateBorder.IsVisible = true;
                });
                break;
            }
            Thread.Sleep(500);
        }
        if (hasTextMouse)
        {
            ChatStack.Dispatcher.Dispatch(() =>
            {
                //移除最后一个字符
                labelAndMessageId.label.Text = labelAndMessageId.label.Text.Remove(labelAndMessageId.label.Text.Length - 1);
            });
        }
    }

    private void InputEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputEditor.Text) || !lastAnswerGenerated)
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

    public void CreateNewDialog()
    {
        LoadDialog(Guid.NewGuid().ToString());
    }

    public void LoadDialog(string dialogId)
    {
        ChatStack.Clear();
        lastQuestion = string.Empty;
        lastAnswerGenerated = true;
        InputEditor.Text = string.Empty;
        CurrentDialogId = dialogId;
        GPTAIDialogManager.SuggestionsReceived -= GPTAIDialogManager_SuggestionsReceived;
        SuggestionFlexLayout.Clear();
        if (GPTAIDialogManager.GetDialog(CurrentDialogId) is null)
        {
            GPTAIDialogManager.SuggestionsReceived += GPTAIDialogManager_SuggestionsReceived;
            SuggestionFlexLayout.IsVisible = true;
            GPTAIDialogManager.GetSuggestionFromGPT();
            return;
        }
        SuggestionFlexLayout.IsVisible = false;
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
            ChatStack.Children.Add(askGrid);
            lastQuestion = chatDialog[i];
            var responseLabel = new Label
            {
                Text = chatDialog[i + 1],
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
            ChatStack.Children.Add(responseGrid);
            labelAndMessageIdList.Add(new LabelAndMessageId(responseLabel, Guid.NewGuid().ToString()));
        }
        ChatScrollView.Dispatcher.Dispatch(() =>
        {
            ChatScrollView.ScrollToAsync(ChatStack, ScrollToPosition.End, false);
        });
        RegenerateBorder.IsVisible = true;
    }

    private void RegenerateButton_Clicked(object sender, EventArgs e)
    {
        if (ChatStack.Count > 0)
        {
            var lastMessage = labelAndMessageIdList.Last();
            lastMessage.label.Text = string.Empty;
            lastMessage.label.TextColor = Color.FromArgb("#D1D5DB");
            lastMessage.messageId = Guid.NewGuid().ToString();
            lastMessage.completeState = false;
            SendButton.IsEnabled = false;
            RegenerateBorder.IsVisible = false;
            StopGenerateBorder.IsVisible = true;
            lastAnswerGenerated = false;
            var currentDialogCollection = memorableXFEChatGPT.MemoryDialog[CurrentDialogId];
            currentDialogCollection.RemoveAt(currentDialogCollection.Count - 1);
            new Action(() => TextMouseUpdate(lastMessage)).StartNewTask();
            memorableXFEChatGPT.AskChatGPT(CurrentDialogId, lastMessage.messageId, lastQuestion);
        }
    }

    private void ChatScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        if (autoScrolled)
        {
            autoScrolled = false;
            autoScrollEnable = true;
        }
        else
        {
            autoScrollEnable = false;
        }
    }

    private void StopGenerateButton_Clicked(object sender, EventArgs e)
    {
        StopGenerateBorder.IsVisible = false;
        RegenerateBorder.IsVisible = true;
        labelAndMessageIdList.Last().completeState = true;
        lastAnswerGenerated = true;
        SendButton.IsEnabled = true;
    }
}
class LabelAndMessageId
{
    public Label label;
    public string messageId;
    public bool completeState = false;
    public LabelAndMessageId(Label label, string messageId)
    {
        this.label = label;
        this.messageId = messageId;
    }
    public LabelAndMessageId(string messageId)
    {
        this.messageId = messageId;
    }
}