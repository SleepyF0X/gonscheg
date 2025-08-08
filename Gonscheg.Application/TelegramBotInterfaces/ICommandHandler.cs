using Gonscheg.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Application.TelegramBotInterfaces;

public interface ICommandHandler
{
    BotCommand Command { get; }
    UserType CommandUserType { get; }
    public Task HandleCommandAsync(ITelegramBotClient botClient, Update update);
}
