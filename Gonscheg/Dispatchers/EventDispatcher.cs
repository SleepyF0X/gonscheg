using Gonscheg.Application.TelegramBotInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Dispatchers;

public class EventDispatcher(
    IServiceScopeFactory scopeFactory,
    IEnumerable<IEventHandler> handlers,
    ILogger<EventDispatcher> logger)
{
    public async Task DispatchAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        foreach (var handlerType in handlers.Select(h => h.GetType()))
        {
            using var scope = scopeFactory.CreateScope();
            var handler = (IEventHandler)scope.ServiceProvider.GetRequiredService(handlerType);

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
