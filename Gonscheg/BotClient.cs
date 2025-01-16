using Gonscheg.Commands;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg;

public class BotClient
{
    public ITelegramBotClient Client { get; set; }
    private static ReceiverOptions? _receiverOptions;
    public BotClient()
    {
        Client = new TelegramBotClient("7813333407:AAHO_MrEEHSviX3TqpKVUfJ1d29BAZkWfp8");
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates =
            [
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
        var commands = new[]
        {
            StartCommand.Command
        };

        await Client.SetMyCommands(commands, cancellationToken: cts.Token);
        Client.StartReceiving(updateHandler, errorHandler, _receiverOptions, cts.Token);
    }
}