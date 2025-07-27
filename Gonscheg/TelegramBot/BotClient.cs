using Gonscheg.Commands;
using Gonscheg.Shared;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.TelegramBot;

public class BotClient
{
    public ITelegramBotClient Client { get; set; }
    private static ReceiverOptions? _receiverOptions;
    public BotClient()
    {
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
        var adminCommands = new[]
        {
            TestCommand.Command,
        };
        var commands = new[]
        {
            StartCommand.Command,
            WeatherCommand.Command,
        };

        await Client.SetMyCommands(adminCommands, BotCommandScope.AllChatAdministrators(), cancellationToken: cts.Token);
        await Client.SetMyCommands(commands, cancellationToken: cts.Token);
        Client.StartReceiving(updateHandler, errorHandler, _receiverOptions, cts.Token);
    }
}