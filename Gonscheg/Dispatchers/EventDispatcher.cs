using Gonscheg.Application.TelegramBotInterfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Dispatchers;

public class EventDispatcher(
    IEnumerable<IEventHandler> handlers,
    ILogger<EventDispatcher> logger)
{
    public async Task DispatchAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleEventAsync(botClient, update, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in handler {handler.GetType().Name}: {ex}");
            }
        }
    }
}
