using Gonscheg.Domain.Enums;

namespace Gonscheg.Domain.Entities;

public class Chat : Entity
{
    public string ChatId { get; set; }
    public ChatType Type { get; set; }
    public string Name { get; set; }

    public Chat(string chatId, string name)
    {
        ChatId = chatId;
        Name = name;
        Type = ChatType.Default;
    }
}