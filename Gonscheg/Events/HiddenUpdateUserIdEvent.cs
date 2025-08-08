using Gonscheg.Application.Repositories;
using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Entities;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Events;

public class HiddenUpdateUserIdEvent(
    IBaseCRUDRepository<ChatUser> userRepository,
    ILogger<HiddenUpdateUserIdEvent> logger) : IEventHandler
{
    public async Task HandleEventAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(update.Message?.Text))
        {
            return;
        }

        var messageUserTag = update.Message.From.Username;

        if (string.IsNullOrWhiteSpace(messageUserTag))
        {
            return;
        }

        var user = await userRepository.GetByAsync(u => u.TelegramTag == messageUserTag);

        if (user is not { TelegramUserId: null })
        {
            return;
        }

        user.TelegramUserId = update.Message.From.Id;

        await userRepository.UpdateAsync(user);

        logger.LogInformation($"UPDATED Telegram User Id for user: {user.GetTag()}");
    }
}
