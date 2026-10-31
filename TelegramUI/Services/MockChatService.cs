using TelegramUI.Models;

namespace TelegramUI.Services;

public class MockChatService
{
    private readonly List<Chat> _chats = new();
    private readonly Dictionary<int, List<Message>> _messages = new();
    private int _nextMessageId = 1000;

    public MockChatService()
    {
        InitializeMockData();
    }

    public List<Chat> GetChats() => _chats;

    public List<Message> GetMessages(int chatId)
    {
        return _messages.TryGetValue(chatId, out var messages) ? messages : new List<Message>();
    }

    public Message SendMessage(int chatId, string content, Message? replyTo = null)
    {
        var message = new Message
        {
            Id = _nextMessageId++,
            ChatId = chatId,
            Content = content,
            Type = MessageType.Text,
            Timestamp = DateTime.Now,
            Status = MessageStatus.Sent,
            IsOutgoing = true,
            ReplyTo = replyTo
        };

        if (_messages.ContainsKey(chatId))
        {
            _messages[chatId].Add(message);
        }

        return message;
    }

    public List<Message> LoadOlderMessages(int chatId, int beforeId, int count = 50)
    {
        if (!_messages.TryGetValue(chatId, out var all)) return new List<Message>();

        var older = all
            .Where(m => m.Id < beforeId)
            .OrderByDescending(m => m.Id)
            .Take(count)
            .OrderBy(m => m.Id)
            .ToList();

        return older;
    }

    private void InitializeMockData()
    {
        var users = CreateMockUsers();
        var chats = CreateMockChats(users);
        
        foreach (var chat in chats)
        {
            _chats.Add(chat);
            _messages[chat.Id] = CreateMockMessages(chat, users);
            
            if (_messages[chat.Id].Count > 0)
            {
                chat.LastMessage = _messages[chat.Id].Last();
            }
        }
    }

    private List<User> CreateMockUsers()
    {
        return new List<User>
        {
            new() { Id = 1, FirstName = "Sarah", LastName = "Anderson", Username = "@sarah", IsOnline = true, IsVerified = true },
            new() { Id = 2, FirstName = "Michael", LastName = "Chen", Username = "@mchen", IsOnline = true },
            new() { Id = 3, FirstName = "Emma", LastName = "Rodriguez", Username = "@emma_r", IsOnline = false, LastSeen = DateTime.Now.AddMinutes(-15) },
            new() { Id = 4, FirstName = "James", LastName = "Wilson", Username = "@jameswilson", IsOnline = true },
            new() { Id = 5, FirstName = "Olivia", LastName = "Martinez", Username = "@olivia", IsOnline = false, LastSeen = DateTime.Now.AddHours(-2) },
            new() { Id = 6, FirstName = "Daniel", LastName = "Taylor", Username = "@dtaylor", IsOnline = true, IsVerified = true },
            new() { Id = 7, FirstName = "Sophia", LastName = "Brown", Username = "@sophia_b", IsOnline = false, LastSeen = DateTime.Now.AddMinutes(-45) },
            new() { Id = 8, FirstName = "Alexander", LastName = "Davis", Username = "@alex", IsOnline = true },
            new() { Id = 9, FirstName = "Mia", LastName = "Johnson", Username = "@mia.j", IsOnline = true, IsVerified = true },
            new() { Id = 10, FirstName = "Ethan", LastName = "Moore", Username = "@ethanm", IsOnline = false, LastSeen = DateTime.Now.AddHours(-1) },
            new() { Id = 11, FirstName = "Charlotte", LastName = "Lewis", Username = "@charlotte", IsOnline = true },
            new() { Id = 12, FirstName = "Benjamin", LastName = "Walker", Username = "@benw", IsOnline = false, LastSeen = DateTime.Now.AddHours(-5) },
            new() { Id = 13, FirstName = "Amelia", LastName = "Hall", Username = "@amelia_h", IsOnline = true },
            new() { Id = 14, FirstName = "Lucas", LastName = "Young", Username = "@lucas.y", IsOnline = false, LastSeen = DateTime.Now.AddDays(-1) },
            new() { Id = 15, FirstName = "Harper", LastName = "King", Username = "@harper", IsOnline = true },
            new() { Id = 16, FirstName = "Elijah", LastName = "Wright", Username = "@elijah", IsOnline = false, LastSeen = DateTime.Now.AddMinutes(-20) },
            new() { Id = 17, FirstName = "Evelyn", LastName = "Scott", Username = "@evelyn.s", IsOnline = true },
            new() { Id = 18, FirstName = "Mason", LastName = "Green", Username = "@mason.g", IsOnline = false, LastSeen = DateTime.Now.AddHours(-3) },
        };
    }

    private List<Chat> CreateMockChats(List<User> users)
    {
        return new List<Chat>
        {
            new()
            {
                Id = 1,
                Title = users[0].FullName,
                Type = ChatType.Private,
                IsOnline = users[0].IsOnline,
                LastActivity = DateTime.Now.AddMinutes(-2),
                UnreadCount = 3,
                IsPinned = true,
                Members = new List<User> { users[0] }
            },
            new()
            {
                Id = 2,
                Title = users[1].FullName,
                Type = ChatType.Private,
                IsOnline = users[1].IsOnline,
                LastActivity = DateTime.Now.AddMinutes(-5),
                UnreadCount = 0,
                IsPinned = true,
                Members = new List<User> { users[1] }
            },
            new()
            {
                Id = 3,
                Title = "Design Team",
                Type = ChatType.Group,
                LastActivity = DateTime.Now.AddMinutes(-10),
                UnreadCount = 12,
                IsPinned = false,
                Members = new List<User> { users[2], users[3], users[4] }
            },
            new()
            {
                Id = 4,
                Title = users[2].FullName,
                Type = ChatType.Private,
                IsOnline = users[2].IsOnline,
                LastActivity = DateTime.Now.AddHours(-1),
                UnreadCount = 0,
                IsPinned = false,
                Members = new List<User> { users[2] }
            },
            new()
            {
                Id = 5,
                Title = "Development Updates",
                Type = ChatType.Channel,
                LastActivity = DateTime.Now.AddHours(-3),
                UnreadCount = 5,
                IsPinned = false,
                IsVerified = true,
                Members = new List<User>()
            },
            new()
            {
                Id = 6,
                Title = users[5].FullName,
                Type = ChatType.Private,
                IsOnline = users[5].IsOnline,
                LastActivity = DateTime.Now.AddHours(-5),
                UnreadCount = 0,
                IsPinned = false,
                IsMuted = true,
                Members = new List<User> { users[5] }
            },
            new()
            {
                Id = 7,
                Title = "Weekend Plans",
                Type = ChatType.Group,
                LastActivity = DateTime.Now.AddDays(-1),
                UnreadCount = 0,
                IsPinned = false,
                Members = new List<User> { users[6], users[7] }
            },
            new()
            {
                Id = 8,
                Title = users[7].FullName,
                Type = ChatType.Private,
                IsOnline = users[7].IsOnline,
                LastActivity = DateTime.Now.AddDays(-2),
                UnreadCount = 0,
                IsPinned = false,
                Draft = "Hey, are you available for",
                Members = new List<User> { users[7] }
            },
            new()
            {
                Id = 9,
                Title = users[8].FullName,
                Type = ChatType.Private,
                IsOnline = users[8].IsOnline,
                LastActivity = DateTime.Now.AddMinutes(-35),
                UnreadCount = 2,
                Members = new List<User> { users[8] }
            },
            new()
            {
                Id = 10,
                Title = "Product Roadmap",
                Type = ChatType.Group,
                LastActivity = DateTime.Now.AddMinutes(-55),
                UnreadCount = 7,
                Members = new List<User> { users[9], users[10], users[12], users[14] }
            },
            new()
            {
                Id = 11,
                Title = users[10].FullName,
                Type = ChatType.Private,
                IsOnline = users[10].IsOnline,
                LastActivity = DateTime.Now.AddHours(-2),
                UnreadCount = 0,
                IsMuted = true,
                Members = new List<User> { users[10] }
            },
            new()
            {
                Id = 12,
                Title = "Tech News",
                Type = ChatType.Channel,
                LastActivity = DateTime.Now.AddHours(-4),
                UnreadCount = 21,
                IsVerified = true,
                Members = new List<User>()
            },
            new()
            {
                Id = 13,
                Title = users[12].FullName,
                Type = ChatType.Private,
                IsOnline = users[12].IsOnline,
                LastActivity = DateTime.Now.AddHours(-6),
                UnreadCount = 0,
                Members = new List<User> { users[12] }
            },
            new()
            {
                Id = 14,
                Title = "Q3 Retrospective",
                Type = ChatType.Group,
                LastActivity = DateTime.Now.AddHours(-9),
                UnreadCount = 0,
                Members = new List<User> { users[13], users[3], users[5] }
            },
            new()
            {
                Id = 15,
                Title = users[14].FullName,
                Type = ChatType.Private,
                IsOnline = users[14].IsOnline,
                LastActivity = DateTime.Now.AddDays(-1),
                UnreadCount = 1,
                Members = new List<User> { users[14] }
            },
            new()
            {
                Id = 16,
                Title = "Family",
                Type = ChatType.Group,
                LastActivity = DateTime.Now.AddDays(-1).AddHours(-3),
                UnreadCount = 4,
                Members = new List<User> { users[15], users[16], users[17] }
            },
            new()
            {
                Id = 17,
                Title = users[16].FullName,
                Type = ChatType.Private,
                IsOnline = users[16].IsOnline,
                LastActivity = DateTime.Now.AddDays(-2),
                UnreadCount = 0,
                Members = new List<User> { users[16] }
            },
            new()
            {
                Id = 18,
                Title = "Freelance Clients",
                Type = ChatType.Group,
                LastActivity = DateTime.Now.AddDays(-3),
                UnreadCount = 0,
                IsMuted = true,
                Members = new List<User> { users[17], users[9], users[11] }
            },
            new()
            {
                Id = 19,
                Title = users[17].FullName,
                Type = ChatType.Private,
                IsOnline = users[17].IsOnline,
                LastActivity = DateTime.Now.AddDays(-4),
                UnreadCount = 0,
                Members = new List<User> { users[17] }
            },
            new()
            {
                Id = 20,
                Title = "Book Club",
                Type = ChatType.Group,
                LastActivity = DateTime.Now.AddDays(-5),
                UnreadCount = 0,
                Members = new List<User> { users[11], users[15] }
            },
        };
    }

    private List<Message> CreateMockMessages(Chat chat, List<User> users)
    {
        var messages = new List<Message>();
        var baseTime = DateTime.Now.AddHours(-24);

        switch (chat.Id)
        {
            case 1:
                // Large history: 800 messages for scroll/virtualization testing
                var rand = new Random(42);
                for (int i = 0; i < 800; i++)
                {
                    var isOut = i % 3 == 0;
                    var sender = isOut ? null : users[rand.Next(users.Count)];
                    string[] pool =
                    {
                        "Hey, how's it going?",
                        "Working on the new UI now. Looks really clean.",
                        "Can you review my latest commit when you get a chance?",
                        "Sure, give me a few minutes.",
                        "The animation timing feels perfect now.",
                        "Let's sync up tomorrow morning about the roadmap.",
                        "This design language is much better than v1.",
                        "Agreed. Spacing and typography need final pass.",
                        "I'll push the dark theme tokens tonight.",
                        "Perfect, see you in the standup.",
                        "سلام! چطوری؟",
                        "این طرح خیلی خوب شده. ممنون!"
                    };
                    messages.Add(CreateMessage(
                        100 + i, chat.Id, sender,
                        pool[rand.Next(pool.Length)],
                        baseTime.AddMinutes(i * 2), isOut));
                }
                break;

            case 2:
                messages.AddRange(new[]
                {
                    CreateMessage(200, chat.Id, null, "Morning! Ready for today's meeting?", baseTime.AddHours(8), true),
                    CreateMessage(201, chat.Id, users[1], "Yes, I've prepared all the slides.", baseTime.AddHours(8).AddMinutes(5), false),
                    CreateMessage(202, chat.Id, null, "Perfect. See you at 10 AM.", baseTime.AddHours(8).AddMinutes(7), true),
                    CreateMessage(203, chat.Id, users[1], "See you then!", baseTime.AddHours(8).AddMinutes(8), false),
                });
                break;

            case 3:
                messages.AddRange(new[]
                {
                    CreateMessage(300, chat.Id, users[2], "Team, we need to finalize the mockups by Friday.", baseTime.AddHours(5), false),
                    CreateMessage(301, chat.Id, users[3], "I'm working on the mobile version right now.", baseTime.AddHours(5).AddMinutes(10), false),
                    CreateMessage(302, chat.Id, users[4], "Desktop version is done. Will share the link soon.", baseTime.AddHours(5).AddMinutes(15), false),
                    CreateMessage(303, chat.Id, null, "Great work everyone! Keep it up.", baseTime.AddHours(5).AddMinutes(20), true),
                    CreateMessage(304, chat.Id, users[2], "Also, don't forget about the user feedback from last sprint.", baseTime.AddHours(6), false),
                });
                break;

            case 4:
                messages.AddRange(new[]
                {
                    CreateMessage(400, chat.Id, users[2], "Hi! How have you been?", baseTime.AddHours(12), false),
                    CreateMessage(401, chat.Id, null, "Doing well, thanks! Busy with work.", baseTime.AddHours(12).AddMinutes(30), true),
                    CreateMessage(402, chat.Id, users[2], "Same here. Let's catch up soon.", baseTime.AddHours(13), false),
                });
                break;

            case 5:
                messages.AddRange(new[]
                {
                    CreateMessage(500, chat.Id, null, "New feature release v2.5 is now live!", baseTime.AddHours(15), false, MessageType.System),
                    CreateMessage(501, chat.Id, null, "Improvements include better performance and bug fixes.", baseTime.AddHours(15).AddMinutes(1), false, MessageType.System),
                });
                break;

            case 9:
                messages.AddRange(new[]
                {
                    CreateMessage(900, chat.Id, users[8], "Hey! Did you get my email?", baseTime.AddHours(9), false),
                    CreateMessage(901, chat.Id, null, "Yes, just saw it. Replying now.", baseTime.AddHours(9).AddMinutes(4), true),
                    CreateMessage(902, chat.Id, users[8], "Great, thanks! 🙏", baseTime.AddHours(9).AddMinutes(6), false),
                });
                break;

            case 10:
                messages.AddRange(new[]
                {
                    CreateMessage(1000, chat.Id, users[9], "Q4 roadmap draft is ready for review.", baseTime.AddHours(7), false),
                    CreateMessage(1001, chat.Id, users[10], "I'll add the mobile milestones today.", baseTime.AddHours(7).AddMinutes(12), false),
                    CreateMessage(1002, chat.Id, users[12], "Can we push the beta to November?", baseTime.AddHours(7).AddMinutes(25), false),
                    CreateMessage(1003, chat.Id, null, "Let's discuss in tomorrow's sync.", baseTime.AddHours(7).AddMinutes(30), true),
                    CreateMessage(1004, chat.Id, users[14], "Sounds good 👍", baseTime.AddHours(7).AddMinutes(32), false),
                });
                break;

            case 11:
                messages.AddRange(new[]
                {
                    CreateMessage(1100, chat.Id, users[10], "Lunch tomorrow?", baseTime.AddHours(11), false),
                    CreateMessage(1101, chat.Id, null, "Sure, 1 PM at the usual place.", baseTime.AddHours(11).AddMinutes(3), true),
                });
                break;

            case 12:
                var randCh12 = new Random(7);
                string[] news =
                {
                    "🚀 .NET 11 performance benchmarks published",
                    "💡 Tip: use Span<T> to reduce allocations",
                    "📦 New WPF community toolkit release",
                    "🔥 Weekly digest: top 10 dev stories",
                    "🛠️ Visual Studio update improves hot reload",
                };
                for (int i = 0; i < 12; i++)
                    messages.Add(CreateMessage(1200 + i, chat.Id, null, news[randCh12.Next(news.Length)], baseTime.AddHours(13).AddMinutes(i * 45), false));
                break;

            case 13:
                messages.AddRange(new[]
                {
                    CreateMessage(1300, chat.Id, users[12], "The invoice for April is attached.", baseTime.AddHours(14), false, MessageType.File),
                    CreateMessage(1301, chat.Id, null, "Received, processing payment this week.", baseTime.AddHours(14).AddMinutes(10), true),
                });
                break;

            case 14:
                messages.AddRange(new[]
                {
                    CreateMessage(1400, chat.Id, users[13], "Retrospective notes are in the shared doc.", baseTime.AddHours(17), false),
                    CreateMessage(1401, chat.Id, users[3], "Action items assigned. Check your boards.", baseTime.AddHours(17).AddMinutes(20), false),
                    CreateMessage(1402, chat.Id, users[5], "Done ✅", baseTime.AddHours(17).AddMinutes(45), false),
                });
                break;

            case 15:
                messages.AddRange(new[]
                {
                    CreateMessage(1500, chat.Id, users[14], "Can you send me the design file when you get a chance?", baseTime.AddHours(20), false),
                });
                break;

            case 16:
                messages.AddRange(new[]
                {
                    CreateMessage(1600, chat.Id, users[15], "Dinner at grandma's this Sunday?", baseTime.AddHours(22), false),
                    CreateMessage(1601, chat.Id, users[16], "We'll be there! 🎉", baseTime.AddHours(22).AddMinutes(15), false),
                    CreateMessage(1602, chat.Id, null, "Count us in.", baseTime.AddHours(22).AddMinutes(20), true),
                    CreateMessage(1603, chat.Id, users[17], "What should I bring?", baseTime.AddHours(22).AddMinutes(40), false),
                });
                break;

            case 17:
                messages.AddRange(new[]
                {
                    CreateMessage(1700, chat.Id, users[16], "Happy birthday!! 🎂", baseTime.AddHours(26), false),
                    CreateMessage(1701, chat.Id, null, "Thank you so much!", baseTime.AddHours(26).AddMinutes(5), true),
                });
                break;

            case 18:
                messages.AddRange(new[]
                {
                    CreateMessage(1800, chat.Id, users[17], "Client feedback on the landing page: approved!", baseTime.AddHours(30), false),
                    CreateMessage(1801, chat.Id, users[9], "Invoice sent.", baseTime.AddHours(30).AddMinutes(30), false),
                });
                break;

            case 19:
                messages.AddRange(new[]
                {
                    CreateMessage(1900, chat.Id, users[17], "Are you joining the conference in March?", baseTime.AddHours(40), false),
                });
                break;

            case 20:
                messages.AddRange(new[]
                {
                    CreateMessage(2000, chat.Id, users[11], "This month's pick: 'The Pragmatic Programmer'.", baseTime.AddHours(48), false),
                    CreateMessage(2001, chat.Id, users[15], "Loved that one. Discussion meetup on the 28th?", baseTime.AddHours(48).AddMinutes(40), false),
                });
                break;
        }

        return messages;
    }

    private Message CreateMessage(int id, int chatId, User? sender, string content, DateTime timestamp, bool isOutgoing, MessageType type = MessageType.Text)
    {
        var msg = new Message
        {
            Id = id,
            ChatId = chatId,
            Sender = sender,
            Content = content,
            Type = type,
            Timestamp = timestamp,
            Status = isOutgoing ? MessageStatus.Read : MessageStatus.Delivered,
            IsOutgoing = isOutgoing
        };

        // Seed reactions on some messages
        if (id % 7 == 0)
            msg.Reactions = new List<Reaction> { new() { Emoji = "👍", Count = 2, IsSelectedByUser = false } };
        if (id % 23 == 0)
            msg.Reactions = new List<Reaction>
            {
                new() { Emoji = "❤️", Count = 4 },
                new() { Emoji = "🔥", Count = 1 }
            };

        return msg;
    }
}
