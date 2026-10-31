using TelegramUI.ViewModels;

namespace TelegramUI.Models;

public enum ChatType
{
    Private,
    Group,
    Channel
}

public class Chat : ViewModelBase
{
    private int _id;
    private string _title = string.Empty;
    private ChatType _type;
    private Message? _lastMessage;
    private int _unreadCount;
    private bool _isPinned;
    private bool _isMuted;
    private bool _isOnline;
    private DateTime? _lastActivity;
    private string? _draft;
    private bool _isVerified;

    public int Id { get => _id; set => SetProperty(ref _id, value); }
    public string Title { get => _title; set => SetProperty(ref _title, value); }
    public ChatType Type { get => _type; set => SetProperty(ref _type, value); }

    public Message? LastMessage
    {
        get => _lastMessage;
        set => SetProperty(ref _lastMessage, value);
    }

    public int UnreadCount
    {
        get => _unreadCount;
        set => SetProperty(ref _unreadCount, value);
    }

    public bool IsPinned { get => _isPinned; set => SetProperty(ref _isPinned, value); }
    public bool IsMuted { get => _isMuted; set => SetProperty(ref _isMuted, value); }
    public bool IsOnline { get => _isOnline; set => SetProperty(ref _isOnline, value); }

    public DateTime? LastActivity
    {
        get => _lastActivity;
        set => SetProperty(ref _lastActivity, value);
    }

    public string? Draft { get => _draft; set => SetProperty(ref _draft, value); }
    public bool IsVerified { get => _isVerified; set => SetProperty(ref _isVerified, value); }
    public List<User> Members { get; set; } = new();
}
