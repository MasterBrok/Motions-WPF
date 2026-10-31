namespace TelegramUI.Models;

public class Reaction
{
    public string Emoji { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsSelectedByUser { get; set; }
}
