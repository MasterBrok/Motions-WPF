using System.Collections.ObjectModel;
using TelegramUI.Models;

namespace TelegramUI.ViewModels;

public class ChatListViewModel : ViewModelBase
{
    private ObservableCollection<Chat> _chats = new();
    private ObservableCollection<Chat> _filteredChats = new();
    private Chat? _selectedChat;
    private string _searchText = string.Empty;
    private string _selectedFilter = "All";

    private static readonly string[] Filters = { "All", "Unread", "Groups" };

    public ObservableCollection<Chat> Chats
    {
        get => _chats;
        set => SetProperty(ref _chats, value);
    }

    public ObservableCollection<Chat> FilteredChats
    {
        get => _filteredChats;
        set => SetProperty(ref _filteredChats, value);
    }

    public Chat? SelectedChat
    {
        get => _selectedChat;
        set => SetProperty(ref _selectedChat, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                FilterChanged?.Invoke();
        }
    }

    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            if (SetProperty(ref _selectedFilter, value))
                FilterChanged?.Invoke();
        }
    }

    public string[] FilterTabs => Filters;

    public ObservableCollection<TelegramUI.Models.User> Contacts { get; } = new();
    public ObservableCollection<CallEntry> Calls { get; } = new();
    public ObservableCollection<TelegramUI.Models.Message> SavedMessages { get; } = new();

    public event Action? FilterChanged;
}

public class CallEntry : ViewModelBase
{
    public string Name { get; set; } = string.Empty;
    public bool IsIncoming { get; set; }
    public bool IsMissed { get; set; }
    public DateTime Time { get; set; }
    public bool IsOnline { get; set; }
}
