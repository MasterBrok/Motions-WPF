using System.Collections.ObjectModel;
using TelegramUI.Models;

namespace TelegramUI.ViewModels;

public class ConversationViewModel : ViewModelBase
{
    private Chat? _currentChat;
    private ObservableCollection<Message> _messages = new();
    private string _messageText = string.Empty;
    private Message? _replyingTo;
    private Message? _lastSentMessage;
    private bool _isTyping;

    public Chat? CurrentChat
    {
        get => _currentChat;
        set => SetProperty(ref _currentChat, value);
    }

    public ObservableCollection<Message> Messages
    {
        get => _messages;
        set => SetProperty(ref _messages, value);
    }

    public string MessageText
    {
        get => _messageText;
        set => SetProperty(ref _messageText, value);
    }

    public Message? ReplyingTo
    {
        get => _replyingTo;
        set => SetProperty(ref _replyingTo, value);
    }

    public Message? LastSentMessage
    {
        get => _lastSentMessage;
        set => SetProperty(ref _lastSentMessage, value);
    }

    public bool IsTyping
    {
        get => _isTyping;
        set => SetProperty(ref _isTyping, value);
    }
}
