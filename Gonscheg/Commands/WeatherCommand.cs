using Gonscheg.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.Commands;

public class WeatherCommand
{
    public static readonly BotCommand Command = new() { Command = "weather", Description = "Weather" };
    private readonly WeatherClient _weatherClient;

    public WeatherCommand(WeatherClient weatherClient)
    {
        _weatherClient = weatherClient;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        var kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kievTimeZone);
        if (update is { Type: UpdateType.Message, Message.Text: "/weather" or "/weather@gonscheg_nelegalniy_bot" })
        {
            var chatId = update.Message.Chat.Id;
            await botClient.SendMessage(
                chatId: chatId,
                text: await _weatherClient.GetFutureDayWeatherAsync(now, "Kyiv")
            );
        }
    }
}