using Telegram.Bot;

namespace Gonscheg.Extensions;

public class PereclichkaExtension
{
    private readonly BotClient _botClient;
    private readonly HashSet<long> _chatIds;
    private Timer? _timer;

    public PereclichkaExtension(BotClient botClient, HashSet<long> chatIds)
    {
        _botClient = botClient;
        _chatIds = chatIds;
    }

    public void StartExtension()
    {
        var kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kievTimeZone);
        var targetTime = DateTime.Today.AddHours(8);

        if (now > targetTime)
        {
            targetTime = targetTime.AddDays(1);
        }

        var initialDelay = targetTime - now;

        _timer = new Timer(
            callback: async _ => await SendMessageAsync(),
            state: null,
            dueTime: initialDelay,
            period: TimeSpan.FromDays(1)
        );
    }

    private async Task SendMessageAsync()
    {
        try
        {
            foreach (var chatId in _chatIds)
            {
                await _botClient.Client.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Ранок потужнi хондаводи!"
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