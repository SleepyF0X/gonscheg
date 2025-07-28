using System.Text.RegularExpressions;
using Gonscheg.Application.Repositories;
using Gonscheg.Domain;
using Gonscheg.Domain.Entities;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.Commands;

public class EditBirthDateCommand(IBaseCRUDRepository<ChatUser> userRepository, ILogger<EditBirthDateCommand> logger)
{
    public static readonly BotCommand Command = new()
        { Command = "ebd", Description = "Edit my birth date (10.08.2001)" };

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        var chat = update.Message.Chat;
        if (update.Type == UpdateType.Message &&
            (update.Message?.Text?.Contains("/ebd") ?? false))
        {
            var messageText = update.Message!.Text;
            var user = await userRepository.GetByAsync(u =>
                           u.TelegramUserId == update.Message.From.Id ||
                           u.TelegramTag == update.Message.From.Username) ??
                       new ChatUser()
                       {
                           TelegramUserId = update.Message.From.Id,
                           TelegramTag = update.Message.From.Username,
                       };
            var date = Regex.Match(
                messageText,
                @"\/ebd(?:@\S+)?\s+(\d{2}\.\d{2}\.\d{4})").Groups[1].Value;
            var isBirthDateValid = DateTime.TryParse(
                date,
                out var birthdate);

            if (isBirthDateValid)
            {
                user.BirthDate = birthdate;

                await userRepository.UpdateAsync(user);
                await botClient.SendMessage(
                    chatId: chat.Id,
                    text: "Дата рождения обновлена"
                );
                logger.LogInformation($"User " +
                                      $"@{user.TelegramTag}\n" +
                                      $"update his birth date to {date}");
            }

            else
            {
                await botClient.SendMessage(
                    chatId: chat.Id,
                    text: "Дата не корректна, формат даты - 10.08.2001"
                );
                logger.LogWarning($"User " +
                                  $"@{user.TelegramTag}\n" +
                                  $"can not update birth date to {date}");
            }
        }
    }
}