using Gonscheg.Commands;
using Gonscheg.Events;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.Handlers;

public static class UpdateHandler
{
    public static HashSet<long> ChatIds { get; set; } = [];
    public static async Task Handle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            HandleCommands(botClient, update);
            await HandleEventsAsync(botClient, update, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }


    // Add Commands handlers
    private static void HandleCommands(ITelegramBotClient botClient, Update update)
    {
        StartCommand.HandleCommand(botClient, update, ChatIds);
        TestCommand.HandleCommand(botClient, update);
    }

    // Add Events handlers
    private static async Task HandleEventsAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await NewMemberEvent.HandleEvent(botClient, update, cancellationToken);
        await IsOurEvent.HandleEvent(botClient, update, cancellationToken);
    }
}