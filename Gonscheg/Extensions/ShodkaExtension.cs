using Gonscheg.Application.Repositories;
using Gonscheg.Domain;
using Gonscheg.TelegramBot;
using Telegram.Bot;

namespace Gonscheg.Extensions;

public class ShodkaExtension(
    BotClient botClient,
    IBaseCRUDRepository<Chat> chatRepository)
{
    private Timer? _timer;

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
        foreach (var chat in await chatRepository.GetAllAsync())
        {
            await botClient.Client.SendMessage(
                chatId: chat.ChatId,
                text: "Миллионы приходят, уходят, не в них счастье. \nСамым важным на свете всегда будут люди в этой комнате, вот здесь, сейчас. \nОсталось договориться о времени."
            );
        }
    }

    public void StopDailyMessage()
    {
        _timer?.Dispose();
    }
}