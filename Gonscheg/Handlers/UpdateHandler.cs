using Gonscheg.Application.Repositories;
using Gonscheg.Commands;
using Gonscheg.Events;
using Gonscheg.Helpers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Chat = Gonscheg.Domain.Chat;
using User = Gonscheg.Domain.User;

namespace Gonscheg.Handlers;

public class UpdateHandler(
    WeatherClient weatherClient,
    IBaseCRUDRepository<User> userRepository,
    IBaseCRUDRepository<Chat> chatRepository,
    StartCommand startCommand,
    TestCommand testCommand,
    WeatherCommand weatherCommand,
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
    }

    // Add Events handlers
    private async Task HandleEventsAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await NewMemberEvent.HandleEventAsync(botClient, update, cancellationToken);
        await new IsOurEvent(userRepository).HandleEventAsync(botClient, update, cancellationToken);
    }
}