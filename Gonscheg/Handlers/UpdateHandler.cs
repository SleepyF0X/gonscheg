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
            HandleCommands(botClient, update, cancellationToken);
            HandleEventsAsync(botClient, update, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }


    // Add Commands handlers
    private static void HandleCommands(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        StartCommand.HandleCommand(botClient, update, ChatIds);
    }

    // Add Events handlers
    private static async void HandleEventsAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await NewMemberEvent.HandleEvent(botClient, update, cancellationToken);
    }
}