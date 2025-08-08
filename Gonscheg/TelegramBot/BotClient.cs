using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Enums;
using Gonscheg.Shared.Shared;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.TelegramBot;

public class BotClient
{
    public ITelegramBotClient Client { get; }
    private static ReceiverOptions? _receiverOptions;
    private static IEnumerable<ICommandHandler> _commandHandlers;
    public BotClient(IEnumerable<ICommandHandler> commandHandlers)
    {
        _commandHandlers = commandHandlers;
        var botToken = EnvironmentVariables.TelegramBotToken;
        if (string.IsNullOrEmpty(botToken))
        {
            throw new Exception("Bot token is empty");
        }
        Client = new TelegramBotClient(botToken);
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [
                UpdateType.Message,
                UpdateType.ChatMember
            ],
            DropPendingUpdates = true,
        };
    }

    public async Task StartClientAsync(
        Func<ITelegramBotClient, Update, CancellationToken, Task> updateHandler,
        Func<ITelegramBotClient, Exception, CancellationToken, Task> errorHandler)
    {
        using var cts = new CancellationTokenSource();

        var adminCommands = _commandHandlers
            .Where(ch => ch.CommandUserType == UserType.Admin)
            .Select(ch => ch.Command);
        var userCommands = _commandHandlers
            .Where(ch => ch.CommandUserType == UserType.Admin)
            .Select(ch => ch.Command);

        await Client.SetMyCommands(adminCommands, BotCommandScope.AllChatAdministrators(), cancellationToken: cts.Token);
        await Client.SetMyCommands(userCommands, cancellationToken: cts.Token);
        Client.StartReceiving(updateHandler, errorHandler, _receiverOptions, cts.Token);
    }
}
