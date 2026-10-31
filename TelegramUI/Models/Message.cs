using TelegramUI.ViewModels;

namespace TelegramUI.Models;

public enum MessageType
{
    Text,
    Image,
    Video,
    File,
    Audio,
    Voice,
    Sticker,
    Location,
    Contact,
    System
}

public enum MessageStatus
{
    Sending,
    Sent,
    Delivered,
    Read,
    Failed
}

public class Message : ViewModelBase
{
    private int _id;
    private int _chatId;
    private User? _sender;
    private string _content = string.Empty;
    private MessageType _type;
    private DateTime _timestamp;
    private MessageStatus _status;
    private bool _isOutgoing;
    private bool _isEdited;
    private List<Reaction> _reactions = new();

    public int Id { get => _id; set => SetProperty(ref _id, value); }
    public int ChatId { get => _chatId; set => SetProperty(ref _chatId, value); }
    public User? Sender { get => _sender; set => SetProperty(ref _sender, value); }
    public string Content { get => _content; set => SetProperty(ref _content, value); }
    public MessageType Type { get => _type; set => SetProperty(ref _type, value); }
    public DateTime Timestamp { get => _timestamp; set => SetProperty(ref _timestamp, value); }

    public MessageStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public bool IsOutgoing { get => _isOutgoing; set => SetProperty(ref _isOutgoing, value); }
    public bool IsEdited { get => _isEdited; set => SetProperty(ref _isEdited, value); }
    public Message? ReplyTo { get; set; }
    public Message? ForwardedFrom { get; set; }

    public List<Reaction> Reactions
    {
        get => _reactions;
        set => SetProperty(ref _reactions, value);
    }

    public Attachment? Attachment { get; set; }

    // Grouping helpers - set when building the message list
    public bool ShowSender { get; set; }
    public bool IsGroupEnd { get; set; }
}
