using System.Text.RegularExpressions;
using Gonscheg.Application.TelegramBotInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Dispatchers;

public class CommandDispatcher(
    IServiceScopeFactory scopeFactory,
    IEnumerable<ICommandHandler> handlers,
    ILogger<CommandDispatcher> logger)
{
    public async Task DispatchAsync(Update update, ITelegramBotClient botClient)
    {
        var textOrCaption = update.Message?.Text ?? update.Message?.Caption;

        if (string.IsNullOrEmpty(textOrCaption) || !textOrCaption.StartsWith("/"))
        {
            return;
        }

        var command = Regex.Match(textOrCaption, @"^\/(\w+)(?:@\w+)?").Groups[1].Value;
        var handlerType = handlers.FirstOrDefault(h => h.Command.Command == command)?.GetType();

        if (handlerType != null)
        {
            using var scope = scopeFactory.CreateScope();
            var handler = (ICommandHandler)scope.ServiceProvider.GetRequiredService(handlerType);

            await handler.HandleCommandAsync(botClient, update);
        }
        else
        {
            logger.LogWarning($"Unknown command was sent by user {update.Message.From!.Username}");
        }
    }
}
