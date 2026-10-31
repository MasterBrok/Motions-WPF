using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TelegramUI.Models;
using TelegramUI.ViewModels;
using System.Threading.Tasks;

namespace TelegramUI;

public partial class MainWindow : Window
{
    private MainViewModel Vm => (MainViewModel)DataContext;
    private bool _isAtBottom = true;

    // Scroll access adapter (ListBox hosts the messages now)
    private ScrollViewer? MessageScroll => MessagesControl.Template?.FindName("PART_ScrollViewer", MessagesControl) as ScrollViewer
        ?? FindVisualChild<ScrollViewer>(MessagesControl, _ => true);

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        Vm.ChatListViewModel.FilterChanged += () => Vm.RefreshFilter();

        Loaded += (_, _) =>
        {
            var sv = MessageScroll;
            if (sv != null) sv.ScrollChanged += MessageScrollViewer_ScrollChanged;
            MessageInput.Focus();
            StartMockIncomingMessages();
        };
    }

    // Mock incoming messages: typing indicator then received message every ~25s
    private System.Windows.Threading.DispatcherTimer? _mockTimer;
    private readonly Random _rand = new();
    private bool _reducedMotion;
    private bool _mockTimerEnabled = true;

    private void StartMockIncomingMessages()
    {
        _mockTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(25) };
        _mockTimer.Tick += (_, _) =>
        {
            var chat = Vm.ConversationViewModel.CurrentChat;
            if (chat == null || chat.Type == ChatType.Channel) return;

            Vm.ConversationViewModel.IsTyping = true;

            var t = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            t.Tick += (_, _) =>
            {
                t.Stop();
                Vm.ConversationViewModel.IsTyping = false;

                if (Vm.ConversationViewModel.CurrentChat != chat) return;

                string[] incoming =
                {
                    "Sounds good to me!",
                    "Let me check and get back to you.",
                    "Did you see the latest update?",
                    "Great job on the release! 🎉",
                    "تایید شد. ممنون",
                    "Can we jump on a quick call?"
                };

                var msg = new Message
                {
                    Id = _rand.Next(10000, 99999),
                    ChatId = chat.Id,
                    Sender = chat.Members.Count > 0 ? chat.Members[0] : new Models.User { FirstName = "Unknown" },
                    Content = incoming[_rand.Next(incoming.Length)],
                    Type = MessageType.Text,
                    Timestamp = DateTime.Now,
                    Status = MessageStatus.Delivered,
                    IsOutgoing = false
                };

                Vm.ConversationViewModel.Messages.Add(msg);
                chat.LastMessage = msg;

                // Unread badge if not at bottom
                if (!_isAtBottom)
                    chat.UnreadCount++;
            };
            t.Start();
        };
        _mockTimer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _mockTimer?.Stop();
        base.OnClosed(e);
    }

    // Window chrome
    private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    // Chat selection - ChatListBox lives inside the view template
    private ListBox? GetChatListBox()
        => GetDescendant(ViewHost, "ChatListBox") as ListBox;

    private static FrameworkElement? GetDescendant(DependencyObject parent, string name)
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is FrameworkElement fe && fe.Name == name) return fe;
            var result = GetDescendant(child, name);
            if (result != null) return result;
        }
        return null;
    }

    private bool _isSelectingChat;

    private async void ChatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var chatLb = GetChatListBox();
        var chat = chatLb?.SelectedItem as Chat;
        if (chat == null || _isSelectingChat) return;
        _isSelectingChat = true;

        LoadingChatOverlay.Visibility = Visibility.Visible;

        try
        {
            // Defer message load so overlay paints first; small delay for perceived load
            var recent = Vm.SelectChatDeferred(chat);
            await Task.Delay(250);
            Vm.ApplyDeferredMessages(recent);
        }
        finally
        {
            LoadingChatOverlay.Visibility = Visibility.Collapsed;
            _isSelectingChat = false;
        }

        EmptyState.Visibility = Vm.ConversationViewModel.Messages.Count == 0
            ? Visibility.Visible : Visibility.Collapsed;
        Dispatcher.InvokeAsync(() =>
        {
            MessageScroll?.ScrollToEnd();
            _isAtBottom = true;
            ScrollToBottomButton.Visibility = Visibility.Collapsed;
            MessageInput.Focus();
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }

    // Filter tabs
    private void FilterTab_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string filter })
        {
            Vm.ChatListViewModel.SelectedFilter = filter;
            Vm.RefreshFilter();
        }
    }

    // Composer keyboard: Enter sends, Shift+Enter ignored (accepts return off)
    private void MessageInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
        {
            e.Handled = true;
            if (Vm.SendMessageCommand.CanExecute(null))
                Vm.SendMessageCommand.Execute(null);
            ScrollToBottomAnimated();
        }
        else if (e.Key == Key.Escape)
        {
            Vm.ConversationViewModel.ReplyingTo = null;
        }
    }

    // Message hover toolbar — fade in/out without layout shift
    private void Message_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is FrameworkElement { Tag: null } && sender is Grid grid)
        {
            var toolbar = FindNameInGrid(grid);
            if (toolbar != null) AnimateToolbar(toolbar, 1.0);
        }
    }

    private void Message_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Grid grid)
        {
            var toolbar = FindNameInGrid(grid);
            if (toolbar != null) AnimateToolbar(toolbar, 0.0);
        }
    }

    private static Border? FindNameInGrid(Grid grid)
        => grid.Children.OfType<Border>().FirstOrDefault(b => b.Name == "MessageToolbar");

    private static void AnimateToolbar(Border toolbar, double to)
    {
        var win = Application.Current?.MainWindow as MainWindow;
        var duration = win?._reducedMotion == true ? TimeSpan.Zero : TimeSpan.FromMilliseconds(140);
        toolbar.BeginAnimation(OpacityProperty,
            new DoubleAnimation(to, duration)
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            });
        toolbar.IsHitTestVisible = to > 0.5;
    }

    // Toolbar actions
    private void ToolbarReply_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Message msg })
            Vm.ConversationViewModel.ReplyingTo = msg;
        MessageInput.Focus();
    }

    private void ToolbarCopy_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Message msg })
            Clipboard.SetText(msg.Content);
    }

    // Reactions
    private void ToolbarReact_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Message msg })
            ToggleReaction(msg, "👍");
    }

    private void ToolbarReactHeart_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Message msg })
            ToggleReaction(msg, "❤️");
    }

    private void ToggleReaction(Message msg, string emoji)
    {
        var existing = msg.Reactions.FirstOrDefault(r => r.Emoji == emoji);
        if (existing == null)
        {
            var updated = new List<Reaction>(msg.Reactions)
            {
                new Reaction { Emoji = emoji, Count = 1, IsSelectedByUser = true }
            };
            msg.Reactions = updated;
        }
        else if (existing.IsSelectedByUser)
        {
            existing.Count--;
            existing.IsSelectedByUser = false;
            if (existing.Count <= 0)
                msg.Reactions = new List<Reaction>(msg.Reactions.Where(r => r.Emoji != emoji));
            else
                msg.Reactions = new List<Reaction>(msg.Reactions);
        }
        else
        {
            existing.Count++;
            existing.IsSelectedByUser = true;
            msg.Reactions = new List<Reaction>(msg.Reactions);
        }
    }

    private void ReactionChip_Click(object sender, MouseButtonEventArgs e)
    {
        var element = (DependencyObject)sender;
        while (element != null)
        {
            if (element is ContentPresenter { DataContext: Message msg1 })
            {
                if (((FrameworkElement)sender).DataContext is Reaction r)
                    ToggleReaction(msg1, r.Emoji);
                return;
            }
            element = System.Windows.Media.VisualTreeHelper.GetParent(element);
        }
    }

    private void ToolbarDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Message msg })
            Vm.ConversationViewModel.Messages.Remove(msg);
    }

    // Reply reference navigation — highlight original
    private void ReplyReference_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border { Tag: Message target })
        {
            var container = FindMessageContainer(target);
            if (container != null)
            {
                container.BringIntoView();
                HighlightMessage(container);
            }
        }
    }

    private FrameworkElement? FindMessageContainer(Message target)
    {
        // Walk item containers to find the one bound to target
        for (int i = 0; i < MessagesControl.Items.Count; i++)
        {
            if (MessagesControl.Items[i] is Message m && m.Id == target.Id)
            {
                var container = (ContentPresenter)MessagesControl.ItemContainerGenerator.ContainerFromIndex(i);
                return container;
            }
        }
        return null;
    }

    private async void HighlightMessage(FrameworkElement container)
    {
        var border = FindVisualChild<Border>(container, b =>
            b.Background is SolidColorBrush && b.ActualWidth > 100);

        if (border == null) return;

        var highlight = Application.Current.TryFindResource("ReplyHighlightBrush") as Brush
            ?? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#443390EC"));
        border.Background = highlight;
        await System.Threading.Tasks.Task.Delay(1200);

        // Restore theme-appropriate color
        bool isOut = (border.DataContext as Message)?.IsOutgoing == true;
        var restoreKey = isOut ? "OutgoingBubbleBrush" : "IncomingBubbleBrush";
        border.SetResourceReference(Border.BackgroundProperty, restoreKey);
    }

    private static T? FindVisualChild<T>(DependencyObject parent, Func<T, bool> predicate) where T : DependencyObject
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typed && predicate(typed)) return typed;
            var result = FindVisualChild(child, predicate);
            if (result != null) return result;
        }
        return null;
    }

    // Scroll behavior
    private bool _isLoadingOlder;

    private async void MessageScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        var sv = (ScrollViewer)sender;
        _isAtBottom = sv.VerticalOffset >= sv.ScrollableHeight - 40;

        ScrollToBottomButton.Visibility = !_isAtBottom ? Visibility.Visible : Visibility.Collapsed;

        if (_isAtBottom && e.ExtentHeightChange > 0)
            sv.ScrollToBottom();

        // Lazy load older messages when scrolled near top
        if (!_isLoadingOlder && sv.VerticalOffset < 60 && Vm.HasMoreMessages && Vm.ConversationViewModel.CurrentChat != null)
        {
            _isLoadingOlder = true;
            LoadingOlderBar.Visibility = Visibility.Visible;
            var prevExtent = sv.ExtentHeight;
            var prevOffset = sv.VerticalOffset;

            var older = Vm.LoadOlder();

            await Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(350);
                // Preserve scroll position: shift offset by height added above
                sv.UpdateLayout();
                var delta = sv.ExtentHeight - prevExtent;
                if (delta > 0 && prevOffset > 0)
                    sv.ScrollToVerticalOffset(prevOffset + delta);
                LoadingOlderBar.Visibility = Visibility.Collapsed;
                _isLoadingOlder = false;
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }
    }

    private void ScrollToBottomAnimated()
    {
        MessageScroll?.ScrollToEnd();
        _isAtBottom = true;
        ScrollToBottomButton.Visibility = Visibility.Collapsed;
    }

    private void Messages_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Route wheel to inner ListBox's own ScrollViewer naturally (default behavior OK)
        e.Handled = false;
    }

    private void ScrollToBottomButton_Click(object sender, RoutedEventArgs e)
    {
        ScrollToBottomAnimated();
    }

    // Header click opens chat info
    private void HeaderInfo_Click(object sender, MouseButtonEventArgs e)
    {
        if (Vm.ConversationViewModel.CurrentChat == null) return;
        var info = new Views.Profile.ChatInfoWindow(Vm.ConversationViewModel.CurrentChat)
        {
            Owner = this,
            Left = Left + ActualWidth - 420,
            Top = Top + 120
        };
        info.Show();
    }

    private void SearchInChatButton_Click(object sender, RoutedEventArgs e)
    {
        StartSearch();
    }

    // Message context menu handlers (DataContext = Message)
    private Message? ContextMessage => (ContextMenu.PlacementTarget as FrameworkElement)?.Tag as Message
        ?? (ContextMenu.PlacementTarget as FrameworkElement)?.DataContext as Message;

    private void MenuMessageReply_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.TemplatedParent is not null && e.Source is FrameworkElement { DataContext: Message msg })
        {
            Vm.ConversationViewModel.ReplyingTo = msg;
            MessageInput.Focus();
        }
    }

    private void MenuMessageCopy_Click(object sender, RoutedEventArgs e)
    {
        if (e.Source is FrameworkElement { DataContext: Message msg })
            Clipboard.SetText(msg.Content);
    }

    private void MenuMessageDelete_Click(object sender, RoutedEventArgs e)
    {
        if (e.Source is FrameworkElement { DataContext: Message msg })
            Vm.ConversationViewModel.Messages.Remove(msg);
    }

    // Sidebar nav
    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string view })
        {
            Vm.CurrentView = view;
            ApplyViewTemplate();
        }
    }

    private void ApplyViewTemplate()
    {
        var key = Vm.CurrentView switch
        {
            "Contacts" => "ContactsViewTemplate",
            "Calls" => "CallsViewTemplate",
            "Saved" => "SavedViewTemplate",
            _ => "ChatsViewTemplate"
        };
        ViewHost.ContentTemplate = (DataTemplate)ViewHost.FindResource(key);
    }

    // Settings button opens settings window
    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settings = new Views.Settings.SettingsWindow(
            Core.ThemeManager.IsDark, _reducedMotion, _mockTimerEnabled)
        {
            Owner = this,
            Left = Left + 60,
            Top = Top + 120
        };
        settings.ReducedMotionChanged += (_, v) => _reducedMotion = v;
        settings.MockTimerChanged += (_, v) =>
        {
            _mockTimerEnabled = v;
            if (_mockTimer != null) _mockTimer.IsEnabled = v;
        };
        settings.Show();
    }

    // Emoji picker
    private static readonly string[] EmojiSet =
    {
        "😀","😃","😄","😁","😆","😅","😂","🤣","😊","😇","🙂","😉","😍","🥰","😘","😋",
        "😜","🤪","🤨","🧐","🤓","😎","🥳","😏","😒","😞","😔","😟","😕","🙁","😣","😖",
        "😫","😩","🥺","😢","😭","😤","😠","😡","🤬","🤯","😳","🥵","🥶","😱","😨","😰",
        "👍","👎","👌","✌️","🤞","🤟","🤘","👏","🙌","🤝","🙏","💪","🔥","⭐","💜","❤️"
    };

    private void EmojiButton_Click(object sender, RoutedEventArgs e)
    {
        if (EmojiList.Items.Count == 0)
            foreach (var emoji in EmojiSet)
                EmojiList.Items.Add(emoji);
        EmojiPopup.IsOpen = !EmojiPopup.IsOpen;
    }

    private void EmojiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (EmojiList.SelectedItem is string emoji)
        {
            Vm.ConversationViewModel.MessageText += emoji;
            EmojiPopup.IsOpen = false;
            EmojiList.SelectedItem = null;
            MessageInput.Focus();
            MessageInput.CaretIndex = MessageInput.Text.Length;
        }
    }

    // Attachment menu (prototype: inserts service-style message)
    private void AttachButton_Click(object sender, RoutedEventArgs e)
    {
        AttachPopup.IsOpen = !AttachPopup.IsOpen;
    }

    private void AttachPhoto_Click(object sender, RoutedEventArgs e) => InsertAttachment("📷 Photo", MessageType.Image);
    private void AttachFile_Click(object sender, RoutedEventArgs e) => InsertAttachment("📄 Document.pdf", MessageType.File);
    private void AttachContact_Click(object sender, RoutedEventArgs e) => InsertAttachment("👤 Contact: Sarah Anderson", MessageType.Contact);
    private void AttachLocation_Click(object sender, RoutedEventArgs e) => InsertAttachment("📍 Location shared", MessageType.Location);

    private void InsertAttachment(string label, MessageType type)
    {
        AttachPopup.IsOpen = false;
        if (Vm.ConversationViewModel.CurrentChat == null) return;

        var message = new Message
        {
            Id = new Random().Next(5000, 9999),
            ChatId = Vm.ConversationViewModel.CurrentChat.Id,
            Content = label,
            Type = type,
            Timestamp = DateTime.Now,
            Status = MessageStatus.Sent,
            IsOutgoing = true
        };

        Vm.ConversationViewModel.Messages.Add(message);
        Vm.ConversationViewModel.CurrentChat.LastMessage = message;
        ScrollToBottomAnimated();
    }

    // Search state
    private List<int> _searchResultIds = new();
    private int _searchIndex = -1;

    public void StartSearch()
    {
        SearchBox.Focus();
        SearchBox.SelectAll();
    }

    public void SearchInChat()
    {
        if (MessageScroll == null) return;
        StartSearch();
    }

    public void ExecuteSearchInChat(string query)
    {
        _searchResultIds.Clear();
        _searchIndex = -1;

        if (string.IsNullOrWhiteSpace(query)) return;

        _searchResultIds = Vm.ConversationViewModel.Messages
            .Where(m => m.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(m => m.Id)
            .ToList();

        if (_searchResultIds.Count > 0)
            NavigateToSearchResult(0);
    }

    private void NavigateToSearchResult(int index)
    {
        if (_searchResultIds.Count == 0) return;
        _searchIndex = (index + _searchResultIds.Count) % _searchResultIds.Count;

        var targetId = _searchResultIds[_searchIndex];
        var target = Vm.ConversationViewModel.Messages.FirstOrDefault(m => m.Id == targetId);
        if (target == null) return;

        var container = FindMessageContainer(target);
        if (container != null)
        {
            container.BringIntoView();
            HighlightMessage(container);
        }
    }

    // Keyboard shortcuts
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.K && Keyboard.Modifiers == ModifierKeys.Control)
        {
            e.Handled = true;
            StartSearch();
        }
        else if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
        {
            e.Handled = true;
            SearchInChat();
        }
        else if (e.Key == Key.Escape && EmojiPopup.IsOpen)
        {
            EmojiPopup.IsOpen = false;
        }
        else if (e.Key == Key.Escape && AttachPopup.IsOpen)
        {
            AttachPopup.IsOpen = false;
        }
    }

    // Search box Enter = search in current chat messages
    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        e.Handled = true;

        var query = SearchBox.Text.Trim();
        if (query.Length == 0) return;

        // First: jump to matching chat, then search inside
        var match = Vm.ChatListViewModel.FilteredChats
            .FirstOrDefault(c => c.Title.Contains(query, StringComparison.OrdinalIgnoreCase));

        if (match != null && Vm.ChatListViewModel.SelectedChat != match)
        {
            Vm.SelectChat(match);
        }

        ExecuteSearchInChat(query);
    }
}
