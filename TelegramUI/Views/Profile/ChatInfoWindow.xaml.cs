using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TelegramUI.Models;

namespace TelegramUI.Views.Profile;

public partial class ChatInfoWindow : Window
{
    private readonly Chat _chat;

    public ChatInfoWindow(Chat chat)
    {
        InitializeComponent();
        _chat = chat;

        var initials = chat.Title.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        AvatarInitials.Text = initials.Length > 0
            ? string.Concat(initials.Select(w => w[0])).ToUpper()[..Math.Min(2, initials.Length)]
            : "?";

        InfoTitle.Text = chat.Title;

        InfoSubtitle.Text = chat.Type switch
        {
            ChatType.Private => chat.IsOnline ? "online" : "last seen recently",
            ChatType.Group => $"{chat.Members.Count + 1} members",
            ChatType.Channel => "channel · subscribers",
            _ => string.Empty
        };

        UpdateNotifyVisual();
        BuildInfoRows();
        BuildMediaGrid();
        BuildMembersList();
    }

    private void BuildInfoRows()
    {
        InfoRows.Items.Clear();

        AddRow("Username", _chat.Type == ChatType.Private
            ? $"@{_chat.Title.ToLower().Replace(" ", "_")}"
            : $"@{_chat.Title.ToLower().Replace(" ", "")}");

        AddRow("Type", _chat.Type.ToString());

        if (_chat.LastMessage != null)
            AddRow("Last message", _chat.LastMessage.Content, truncate: 60);

        if (_chat.Members.Count > 0)
            AddRow("Members", string.Join(", ", _chat.Members.Select(m => m.FullName)), truncate: 80);

        AddRow("Notifications", _chat.IsMuted ? "Muted" : "Enabled");
    }

    private void AddRow(string label, string value, int truncate = int.MaxValue)
    {
        var panel = new StackPanel { Margin = new Thickness(0, 0, 0, 18) };

        var l = new TextBlock
        {
            Text = label,
            FontSize = 11,
            Margin = new Thickness(4, 0, 0, 3)
        };
        l.SetResourceReference(TextBlock.ForegroundProperty, "MutedTextBrush");

        var v = new TextBlock
        {
            Text = value.Length > truncate ? value[..truncate] + "…" : value,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(4, 0, 0, 0)
        };
        v.SetResourceReference(TextBlock.ForegroundProperty, "PrimaryTextBrush");

        var divider = new Rectangle { Height = 1, Fill = GetBrush("DividerBrush"), Margin = new Thickness(0, 14, 0, 0) };

        panel.Children.Add(l);
        panel.Children.Add(v);
        panel.Children.Add(divider);
        InfoRows.Items.Add(panel);
    }

    private void BuildMediaGrid()
    {
        var rnd = new Random(7);
        var types = new[] { "🖼️", "🌄", "🏞️", "🌅", "📊", "📎" };
        for (int i = 0; i < 18; i++)
        {
            var cell = new Border
            {
                Height = 100,
                Margin = new Thickness(2),
                CornerRadius = new CornerRadius(8),
                Background = GetBrush("HoverSurfaceBrush")
            };
            var label = new TextBlock
            {
                Text = types[rnd.Next(types.Length)],
                FontSize = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            cell.Child = label;
            MediaGrid.Items.Add(cell);
        }
    }

    private void BuildMembersList()
    {
        MembersList.Items.Clear();

        if (_chat.Type == ChatType.Channel)
        {
            MembersList.Items.Add(new TextBlock
            {
                Text = "Member list is not available for channels.",
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(4, 8, 0, 0)
            });
            ((TextBlock)MembersList.Items[^1]).SetResourceReference(TextBlock.ForegroundProperty, "MutedTextBrush");
            return;
        }

        // "You" row first for groups
        if (_chat.Type == ChatType.Group)
            AddMemberRow("You", "owner", online: true);

        foreach (var m in _chat.Members)
            AddMemberRow(m.FullName, m.IsOnline ? "online" : "last seen recently", m.IsOnline);

        if (_chat.Members.Count == 0 && _chat.Type == ChatType.Private)
        {
            MembersList.Items.Add(new TextBlock
            {
                Text = "Direct conversation — no members list.",
                FontSize = 12,
                Margin = new Thickness(4, 8, 0, 0)
            });
            ((TextBlock)MembersList.Items[^1]).SetResourceReference(TextBlock.ForegroundProperty, "MutedTextBrush");
        }
    }

    private void AddMemberRow(string name, string status, bool online)
    {
        var row = new Grid { Margin = new Thickness(0, 6, 0, 6) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var avatar = new Border
        {
            Width = 38, Height = 38,
            CornerRadius = new CornerRadius(999),
            Background = GetBrush("PrimaryAccentBrush"),
            Margin = new Thickness(0, 0, 12, 0)
        };
        var initials = new TextBlock
        {
            Text = name == "You" ? "YOU" : name.Split(' ')[0][..1].ToUpper(),
            FontSize = name == "You" ? 10 : 14,
            FontWeight = FontWeights.Medium
        };
        initials.SetResourceReference(TextBlock.ForegroundProperty, "OnAccentBrush");
        avatar.Child = initials;
        Grid.SetColumn(avatar, 0);

        var info = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        var nameTb = new TextBlock { Text = name, FontSize = 14, FontWeight = FontWeights.SemiBold };
        nameTb.SetResourceReference(TextBlock.ForegroundProperty, "PrimaryTextBrush");

        var statusTb = new TextBlock { Text = status, FontSize = 11, Margin = new Thickness(0, 1, 0, 0) };
        if (online)
            statusTb.SetResourceReference(TextBlock.ForegroundProperty, "PrimaryAccentBrush");
        else
            statusTb.SetResourceReference(TextBlock.ForegroundProperty, "SecondaryTextBrush");

        info.Children.Add(nameTb);
        info.Children.Add(statusTb);
        Grid.SetColumn(info, 1);

        row.Children.Add(avatar);
        row.Children.Add(info);
        MembersList.Items.Add(row);
    }

    private Brush GetBrush(string key)
        => (Brush)(Application.Current.TryFindResource(key) ?? Brushes.Gray);

    private void UpdateNotifyVisual()
        => NotifyLabel.Text = _chat.IsMuted ? "Muted" : "Enabled";

    private void Notify_Click(object sender, RoutedEventArgs e)
    {
        _chat.IsMuted = !_chat.IsMuted;
        UpdateNotifyVisual();
    }

    private void SearchInChat_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Tab_Checked(object sender, RoutedEventArgs e)
    {
        if (InfoTab == null) return; // XAML initialization order

        var tab = ((RadioButton)sender).Content.ToString();
        InfoTab.Visibility = tab == "Info" ? Visibility.Visible : Visibility.Collapsed;
        MediaTab.Visibility = tab == "Media" ? Visibility.Visible : Visibility.Collapsed;
        MembersTab.Visibility = tab == "Members" ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }
}
