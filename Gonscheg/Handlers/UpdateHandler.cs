using Gonscheg.Commands;
using Gonscheg.Events;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Handlers;

public class UpdateHandler(
    StartCommand startCommand,
    TestCommand testCommand,
    WeatherCommand weatherCommand,
    RegisterCommand registerCommand,
    EditBirthDateCommand editBirthDateCommand,
    IsOurEvent isOurEvent,
    NewMemberEvent newMemberEvent,
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
        await startCommand.HandleCommandAsync(botClient, update);
        await testCommand.HandleCommandAsync(botClient, update);
        await weatherCommand.HandleCommandAsync(botClient, update);
        await registerCommand.HandleCommandAsync(botClient, update);
        await editBirthDateCommand.HandleCommandAsync(botClient, update);
    }

    // Add Events handlers
    private async Task HandleEventsAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await newMemberEvent.HandleEventAsync(botClient, update, cancellationToken);
        await isOurEvent.HandleEventAsync(botClient, update, cancellationToken);
    }
}