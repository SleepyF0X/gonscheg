using Gonscheg.Application.Repositories;
using Gonscheg.Domain;
using Gonscheg.Domain.Entities;
using Gonscheg.Helpers;
using Gonscheg.TelegramBot;
using Microsoft.Extensions.Logging;
using Serilog;
using Telegram.Bot;

namespace Gonscheg.Extensions;

public class MorningExtension(
    BotClient botClient,
    WeatherClient weatherClient,
    IBaseCRUDRepository<Chat> chatRepository,
    ILogger<MorningExtension> logger)
{
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

        new Timer(
            callback: async _ => await SendMessageAsync(now),
            state: null,
            dueTime: initialDelay,
            period: TimeSpan.FromDays(1)
        );
    }

    private async Task SendMessageAsync(DateTime now)
    {
        foreach (var chat in await chatRepository.GetAllAsync())
        {
            await botClient.Client.SendTextMessageAsync(
                chatId: chat.ChatId,
                text: "Ранок потужнi хондаводи!"
            );
            await botClient.Client.SendTextMessageAsync(
                chatId: chat.ChatId,
                text: await weatherClient.GetFutureDayWeatherAsync(now, "Kyiv")
            );

            logger.LogInformation($"MORNING message to chat" +
                                  $"{chat.Name}" +
                                  $"with id: {chat.ChatId}" +
                                  $"SUCCESSFULLY SENT");
        }
    }
}