using System.Collections.ObjectModel;
using System.Windows.Input;
using TelegramUI.Core;
using TelegramUI.Models;
using TelegramUI.Services;

namespace TelegramUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly MockChatService _chatService;
    private bool _isDarkTheme = true;

    public ChatListViewModel ChatListViewModel { get; }
    public ConversationViewModel ConversationViewModel { get; }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
                ThemeManager.Apply(value);
        }
    }

    public ICommand SendMessageCommand { get; }
    public ICommand ToggleThemeCommand { get; }
    public ICommand TogglePinCommand { get; }
    public ICommand ToggleMuteCommand { get; }
    public ICommand MarkReadCommand { get; }
    public ICommand DeleteChatCommand { get; }
    public ICommand CancelReplyCommand { get; }
    public event Action? ScrollToBottomRequested;

    private string _currentView = "Chats";
    public string CurrentView
    {
        get => _currentView;
        set
        {
            if (SetProperty(ref _currentView, value))
            {
                OnPropertyChanged(nameof(IsChatsView));
                OnPropertyChanged(nameof(IsContactsView));
                OnPropertyChanged(nameof(IsCallsView));
                OnPropertyChanged(nameof(IsSavedView));
            }
        }
    }

    public bool IsChatsView => CurrentView == "Chats";
    public bool IsContactsView => CurrentView == "Contacts";
    public bool IsCallsView => CurrentView == "Calls";
    public bool IsSavedView => CurrentView == "Saved";

    public MainViewModel()
    {
        _chatService = new MockChatService();

        ChatListViewModel = new ChatListViewModel();
        ConversationViewModel = new ConversationViewModel();

        SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
        ToggleThemeCommand = new RelayCommand(() => IsDarkTheme = !IsDarkTheme);
        TogglePinCommand = new RelayCommand<Chat>(TogglePin);
        ToggleMuteCommand = new RelayCommand<Chat>(ToggleMute);
        MarkReadCommand = new RelayCommand<Chat>(MarkRead);
        DeleteChatCommand = new RelayCommand<Chat>(DeleteChat);
        CancelReplyCommand = new RelayCommand(() => ConversationViewModel.ReplyingTo = null);

        LoadChats();
    }

    private void LoadChats()
    {
        foreach (var chat in _chatService.GetChats())
            ChatListViewModel.Chats.Add(chat);

        ApplyChatFilter();
        LoadSideViews();

        if (ChatListViewModel.Chats.Count > 0)
            SelectChat(ChatListViewModel.Chats[0]);
    }

    private void LoadSideViews()
    {
        // Contacts: unique users across chats
        var seen = new HashSet<int>();
        foreach (var chat in ChatListViewModel.Chats)
        {
            foreach (var member in chat.Members)
            {
                if (seen.Add(member.Id))
                    ChatListViewModel.Contacts.Add(member);
            }
        }

        // Calls mock
        var now = DateTime.Now;
        CallEntry[] calls =
        {
            new() { Name = "Sarah Anderson", IsIncoming = true, IsMissed = false, Time = now.AddMinutes(-40), IsOnline = true },
            new() { Name = "Michael Chen", IsIncoming = false, IsMissed = false, Time = now.AddHours(-3) },
            new() { Name = "Emma Rodriguez", IsIncoming = true, IsMissed = true, Time = now.AddHours(-8) },
            new() { Name = "James Wilson", IsIncoming = false, IsMissed = true, Time = now.AddDays(-1), IsOnline = true },
            new() { Name = "Olivia Martinez", IsIncoming = true, IsMissed = false, Time = now.AddDays(-1).AddHours(-4), IsOnline = true },
            new() { Name = "Daniel Taylor", IsIncoming = false, IsMissed = false, Time = now.AddDays(-2) },
            new() { Name = "Sophia Brown", IsIncoming = true, IsMissed = false, Time = now.AddDays(-2).AddHours(-6) },
            new() { Name = "Alexander Davis", IsIncoming = false, IsMissed = true, Time = now.AddDays(-3) },
            new() { Name = "Design Team", IsIncoming = true, IsMissed = false, Time = now.AddDays(-3).AddHours(-2) },
            new() { Name = "Sarah Anderson", IsIncoming = false, IsMissed = false, Time = now.AddDays(-4).AddHours(-1) },
            new() { Name = "Michael Chen", IsIncoming = true, IsMissed = true, Time = now.AddDays(-4).AddHours(-9) },
            new() { Name = "Emma Rodriguez", IsIncoming = false, IsMissed = false, Time = now.AddDays(-5).AddHours(-3) },
        };
        foreach (var call in calls)
            ChatListViewModel.Calls.Add(call);

        // Saved messages mock
        Message[] saved =
        {
            new() { Id = 9001, Content = "Design tokens reference: https://figma.com/file/tokens", Type = MessageType.Text, Timestamp = now.AddDays(-2), IsOutgoing = true, Status = MessageStatus.Read },
            new() { Id = 9002, Content = "Remember: review PR #142 before Friday standup", Type = MessageType.Text, Timestamp = now.AddHours(-20), IsOutgoing = true, Status = MessageStatus.Read },
            new() { Id = 9003, Content = "آدرس دفتر جدید: خیابان ولیعصر، پلاک ۱۲۰", Type = MessageType.Text, Timestamp = now.AddHours(-14), IsOutgoing = true, Status = MessageStatus.Read },
            new() { Id = 9004, Content = "Wi-Fi password: office-2026!", Type = MessageType.Text, Timestamp = now.AddHours(-10), IsOutgoing = true, Status = MessageStatus.Read },
            new() { Id = 9005, Content = "Checklist for release:\n1. Update changelog\n2. Bump version\n3. Tag build", Type = MessageType.Text, Timestamp = now.AddHours(-6), IsOutgoing = true, Status = MessageStatus.Read },
            new() { Id = 9006, Content = "Invoice_2026_04.pdf", Type = MessageType.File, Timestamp = now.AddHours(-4), IsOutgoing = true, Status = MessageStatus.Read },
            new() { Id = 9007, Content = "Wireframes v3", Type = MessageType.Image, Timestamp = now.AddHours(-2), IsOutgoing = true, Status = MessageStatus.Read },
            new() { Id = 9008, Content = "Color palette: #3390EC primary, #43B581 success, #E35D6A error", Type = MessageType.Text, Timestamp = now.AddMinutes(-50), IsOutgoing = true, Status = MessageStatus.Read },
        };
        foreach (var msg in saved)
            ChatListViewModel.SavedMessages.Add(msg);
    }

    private void ApplyChatFilter()
    {
        var filter = ChatListViewModel.SelectedFilter;
        var search = ChatListViewModel.SearchText?.Trim() ?? string.Empty;

        ChatListViewModel.FilteredChats.Clear();
        var query = ChatListViewModel.Chats.AsEnumerable();

        query = filter switch
        {
            "Unread" => query.Where(c => c.UnreadCount > 0),
            "Groups" => query.Where(c => c.Type == Models.ChatType.Group || c.Type == Models.ChatType.Channel),
            _ => query
        };

        if (search.Length > 0)
            query = query.Where(c => c.Title.Contains(search, StringComparison.OrdinalIgnoreCase));

        foreach (var chat in query.OrderByDescending(c => c.IsPinned).ThenByDescending(c => c.LastActivity))
            ChatListViewModel.FilteredChats.Add(chat);
    }

    public void SelectChat(Chat chat)
    {
        if (chat == null) return;

        ChatListViewModel.SelectedChat = chat;
        ConversationViewModel.CurrentChat = chat;
        ConversationViewModel.ReplyingTo = null;

        var messages = _chatService.GetMessages(chat.Id);
        var recent = messages.Count > 100 ? messages.Skip(messages.Count - 100).ToList() : messages;

        ConversationViewModel.Messages.Clear();
        foreach (var message in recent)
            ConversationViewModel.Messages.Add(message);

        OldestLoadedId = recent.Count > 0 ? recent[0].Id : 0;
        HasMoreMessages = messages.Count > 100;

        if (chat.UnreadCount > 0)
        {
            chat.UnreadCount = 0;
            ApplyChatFilter();
        }
    }

    public List<Message> SelectChatDeferred(Chat chat)
    {
        if (chat == null) return new List<Message>();

        ChatListViewModel.SelectedChat = chat;
        ConversationViewModel.CurrentChat = chat;
        ConversationViewModel.ReplyingTo = null;

        var messages = _chatService.GetMessages(chat.Id);
        var recent = messages.Count > 100 ? messages.Skip(messages.Count - 100).ToList() : messages;

        OldestLoadedId = recent.Count > 0 ? recent[0].Id : 0;
        HasMoreMessages = messages.Count > 100;

        if (chat.UnreadCount > 0)
            chat.UnreadCount = 0;

        return recent;
    }

    public void ApplyDeferredMessages(List<Message> recent)
    {
        ConversationViewModel.Messages.Clear();
        foreach (var message in recent)
            ConversationViewModel.Messages.Add(message);
        ApplyChatFilter();
    }

    public int OldestLoadedId { get; private set; }
    public bool HasMoreMessages { get; private set; }

    public List<Message> LoadOlder()
    {
        if (ConversationViewModel.CurrentChat == null || !HasMoreMessages)
            return new List<Message>();

        var older = _chatService.LoadOlderMessages(ConversationViewModel.CurrentChat.Id, OldestLoadedId);
        HasMoreMessages = older.Count == 50;

        if (older.Count > 0)
        {
            OldestLoadedId = older[0].Id;
            // Insert at beginning preserving existing order
            for (int i = older.Count - 1; i >= 0; i--)
                ConversationViewModel.Messages.Insert(0, older[i]);
        }

        return older;
    }

    private bool CanSendMessage()
        => !string.IsNullOrWhiteSpace(ConversationViewModel.MessageText)
           && ConversationViewModel.CurrentChat != null;

    private void SendMessage()
    {
        if (ConversationViewModel.CurrentChat == null) return;

        var message = _chatService.SendMessage(
            ConversationViewModel.CurrentChat.Id,
            ConversationViewModel.MessageText,
            ConversationViewModel.ReplyingTo
        );

        ConversationViewModel.Messages.Add(message);
        ConversationViewModel.MessageText = string.Empty;

        var reply = ConversationViewModel.ReplyingTo;
        ConversationViewModel.ReplyingTo = null;

        ConversationViewModel.CurrentChat.LastMessage = message;
        ConversationViewModel.LastSentMessage = message;
    }

    private void TogglePin(Chat? chat)
    {
        if (chat == null) return;
        chat.IsPinned = !chat.IsPinned;
        ApplyChatFilter();
    }

    private void ToggleMute(Chat? chat)
    {
        if (chat == null) return;
        chat.IsMuted = !chat.IsMuted;
    }

    private void MarkRead(Chat? chat)
    {
        if (chat == null) return;
        chat.UnreadCount = 0;
        ApplyChatFilter();
    }

    private void DeleteChat(Chat? chat)
    {
        if (chat == null) return;
        ChatListViewModel.Chats.Remove(chat);
        if (ChatListViewModel.SelectedChat == chat && ChatListViewModel.Chats.Count > 0)
            SelectChat(ChatListViewModel.Chats[0]);
        ApplyChatFilter();
    }

    public void RefreshFilter() => ApplyChatFilter();

    public void RequestScrollToBottom() => ScrollToBottomRequested?.Invoke();
}
