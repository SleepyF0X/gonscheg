namespace Gonscheg.Domain;

public class Chat : Entity
{
    public string ChatId { get; set; }

    public Chat(string chatId)
    {
        ChatId = chatId;
    }
}