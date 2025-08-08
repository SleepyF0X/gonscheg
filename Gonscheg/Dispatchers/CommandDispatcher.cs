using Gonscheg.Application.TelegramBotInterfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Dispatchers;

public class CommandDispatcher(
    IEnumerable<ICommandHandler> handlers,
    ILogger<CommandDispatcher> logger)
{
    public async Task DispatchAsync(Update update, ITelegramBotClient botClient)
    {
        if (update.Message?.Text == null || !update.Message.Text.StartsWith("/"))
        {
            return;
        }

        var command = update.Message.Text.Split(' ', 2)[0].TrimStart('/').Split('@')[0];
        var handler = handlers.FirstOrDefault(h => h.Command.Command == command);
        if (handler != null)
        {
            await handler.HandleCommandAsync(botClient, update);
        }
        else
        {
            logger.LogWarning($"Unknown command was sent by user {update.Message.From!.Username}");
        }
    }
}
