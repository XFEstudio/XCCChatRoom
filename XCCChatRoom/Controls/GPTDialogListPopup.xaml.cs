using MauiPopup.Views;
using Microsoft.Maui.Controls.Shapes;
using XCCChatRoom.AllImpl;
using XCCChatRoom.ViewPage;
using XFEExtension.NetCore.FormatExtension;

namespace XCCChatRoom.Controls;

public partial class GPTDialogListPopup : BasePopupPage
{
    public GPTDialogListPopup(string currentDialogID)
    {
        Border currentChoseBorder = null;
        InitializeComponent();
        foreach (var entry in GPTAIDialogManager.XFEEntries)
        {
            Console.WriteLine(entry.ToString());
            if (noneDialogLabel.IsVisible)
                noneDialogLabel.IsVisible = false;
            var contentEntry = new XFEDictionary(entry.Content);
            var titleLabel = new Label
            {
                Text = contentEntry["Title"] == string.Empty ? "无标题对话" : contentEntry["Title"],
                TextColor = Color.Parse("White"),
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation,
                MaximumWidthRequest = 180
            };
            var versionLabel = new Label
            {
                Text = contentEntry["GPTVersion"],
                FontSize = 20,
                TextColor = Color.Parse("#565865"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Shadow = new Shadow
                {
                    Brush = Color.Parse("#565865"),
                    Radius = 5,
                }
            };
            var versionBorder = new Border
            {
                StrokeThickness = 0,
                Margin = new Thickness(5),
                Padding = new Thickness(5, 0),
                BackgroundColor = Color.Parse("White"),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 30
                },
                Content = versionLabel
            };
            var entryGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(180, GridUnitType.Absolute)},
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Auto)},
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Auto)}
                }
            };
            var entryBorder = new Border
            {
                StrokeThickness = 0,
                Margin = new Thickness(10, 5),
                Padding = new Thickness(5),
                BackgroundColor = Color.FromArgb("#565865"),
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 10
                },
                Content = entryGrid
            };
            entryBorder.GestureRecognizers.Add(new TapGestureRecognizer
            {
                CommandParameter = new { Id = entry.Header, Border = entryBorder },
                Command = new Command(sender =>
                {
                    var border = ((dynamic)sender).Border as Border;
                    if (border is not null)
                    {
                        if (currentChoseBorder != border)
                        {
                            border.BackgroundColor = Color.Parse("White");
                            var comTitleLabel = (border.Content as Grid).Children[0] as Label;
                            var comVersionBorder = (border.Content as Grid).Children[1] as Border;
                            comTitleLabel.TextColor = Color.FromArgb("#565865");
                            comVersionBorder.BackgroundColor = Color.FromArgb("#565865");
                            (comVersionBorder.Content as Label).TextColor = Color.Parse("White");
                            if (currentChoseBorder is not null)
                            {
                                currentChoseBorder.BackgroundColor = Color.FromArgb("#565865");
                                var currentTitleLabel = ((currentChoseBorder.Content as Grid).Children[0] as Label);
                                var currentVersionBorder = ((currentChoseBorder.Content as Grid).Children[1] as Border);
                                currentTitleLabel.TextColor = Color.Parse("White");
                                currentVersionBorder.BackgroundColor = Color.Parse("White");
                                (currentVersionBorder.Content as Label).TextColor = Color.FromArgb("#565865");
                            }
                            currentChoseBorder = border;
                        }
                    }
                    GPTAIChatPage.Current.LoadDialog(((dynamic)sender).Id);
                })
            });
            var deleteButton = new ImageButton
            {
                Source = "trash2",
                Margin = new Thickness(5, 0, 3, 0),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40,
                HeightRequest = 40,
                CommandParameter = new { Id = entry.Header, Border = entryBorder },
                Command = new Command(async sender =>
                {
                    if (await Shell.Current?.DisplayAlert("删除", "是否删除该对话？", "删除", "取消"))
                    {
                        var border = ((dynamic)sender).Border as Border;
                        var id = ((dynamic)sender).Id as string;
                        GPTAIDialogManager.XFEEntries.Remove(id);
                        GPTAIDialogManager.SaveDialogs();
                        dialogStackLayout.Children.Remove(border);
                        if (dialogStackLayout.Children.Count == 0)
                            noneDialogLabel.IsVisible = true;
                        if (currentDialogID == id)
                        {
                            currentDialogID = string.Empty;
                            GPTAIChatPage.Current.CreateNewDialog();
                        }
                    }
                })
            };
            entryGrid.Add(titleLabel, 0, 0);
            entryGrid.Add(versionBorder, 1, 0);
            entryGrid.Add(deleteButton, 2, 0);
            if (entry.Header == currentDialogID)
            {
                currentChoseBorder = entryBorder;
                entryBorder.BackgroundColor = Color.Parse("White");
                titleLabel.TextColor = Color.Parse("#565865");
                versionLabel.TextColor = Color.Parse("#565865");
            }
            this.Dispatcher.Dispatch(() =>
            {
                dialogStackLayout.Children.Add(entryBorder);
            });
        }
    }

}