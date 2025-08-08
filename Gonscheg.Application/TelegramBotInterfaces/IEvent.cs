using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Application.TelegramBotInterfaces;

public interface IEventHandler
{
    Task HandleEventAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
}
