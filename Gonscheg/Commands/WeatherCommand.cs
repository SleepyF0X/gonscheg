using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Enums;
using Gonscheg.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Commands;

public class WeatherCommand(
    WeatherClient weatherClient)
    : ICommandHandler
{
    public BotCommand Command => new() { Command = "weather", Description = "Weather" };
    public UserType CommandUserType => UserType.Default;

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        var chatId = update.Message.Chat.Id;
        await botClient.SendMessage(
            chatId: chatId,
            text: await weatherClient.GetDayWeatherMessageAsync("Europe/Kyiv")
        );
    }
}
