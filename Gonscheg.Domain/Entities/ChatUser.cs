namespace Gonscheg.Domain.Entities;

public class ChatUser : Entity
{
    public long? TelegramUserId { get; set; }
    public string? Plate { get; set; }
    public string? TelegramTag { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? VinCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Description { get; set; }
    public string? CarPhotoPath { get; set; }
    public DateTime RegisterDate { get; set; }

    public string GetTag()
    {
        if (!string.IsNullOrWhiteSpace(TelegramTag))
        {
            return $"@{TelegramTag}";
        }
        if (TelegramUserId != null && !string.IsNullOrWhiteSpace(Name))
        {
            return $"[{Name}](tg://user?id={TelegramUserId})";
        }

        return Name;
    }
}
