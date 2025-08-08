using Gonscheg.Dispatchers;
using Gonscheg.Events;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Handlers;

public class UpdateHandler(
    CommandDispatcher commandDispatcher,
    EventDispatcher eventDispatcher,
    ILogger<UpdateHandler> logger)
{

    public async Task Handle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            await HandleCommandsAsync(botClient, update);
            await HandleEventsAsync(botClient, update, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }


    // Add Commands handlers
    private async Task HandleCommandsAsync(ITelegramBotClient botClient, Update update)
    {
        await commandDispatcher.DispatchAsync(update, botClient);
    }

    // Add Events handlers
    private async Task HandleEventsAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await eventDispatcher.DispatchAsync(botClient, update, cancellationToken);
    }
}
