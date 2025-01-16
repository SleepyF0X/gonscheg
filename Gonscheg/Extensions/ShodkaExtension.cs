using Telegram.Bot;

namespace Gonscheg.Extensions;

public class ShodkaExtension
{
    private readonly BotClient _botClient;
    private readonly HashSet<long> _chatIds;
    private Timer? _timer;

    public ShodkaExtension(BotClient botClient, HashSet<long> chatIds)
    {
        _botClient = botClient;
        _chatIds = chatIds;
    }

    public void StartExtension()
    {
        var kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kievTimeZone);

        var targetTime = DateTime.Today.DayOfWeek switch
        {
            DayOfWeek.Friday => DateTime.Today.AddHours(12),
            DayOfWeek.Saturday => DateTime.Today.AddDays(6).AddHours(12),
            _ => DateTime.Today.AddDays(DayOfWeek.Friday - DateTime.Today.DayOfWeek).AddHours(12)
        };

        var initialDelay = targetTime - now;

        _timer = new Timer(
            callback: async _ => await SendMessageAsync(),
            state: null,
            dueTime: initialDelay,
            period: TimeSpan.FromDays(7)
        );
    }

    private async Task SendMessageAsync()
    {
        try
        {
            foreach (var chatId in _chatIds)
            {
                await _botClient.Client.SendMessage(
                    chatId: chatId,
                    text: "Миллионы приходят, уходят, не в них счастье. \nСамым важным на свете всегда будут люди в этой комнате, вот здесь, сейчас. \nОсталось договориться о времени."
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
        }
    }

    public void StopDailyMessage()
    {
        _timer?.Dispose();
    }
}