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

    public ChatUser(string? plate = null, string? telegramTag = null, string? name = null, string? surname = null,
        string? vinCode = null, DateTime? birthDate = null, string? description = null)
    {
        Plate = plate;
        TelegramTag = telegramTag;
        Name = name;
        Surname = surname;
        VinCode = vinCode;
        BirthDate = birthDate;
        Description = description;
    }
}