using System.Text.RegularExpressions;
using Gonscheg.Application.Repositories;
using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Entities;
using Gonscheg.Domain.Enums;
using Gonscheg.Helpers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Commands;

public class EditBirthDateCommand(
    IBaseCRUDRepository<ChatUser> userRepository,
    ILogger<EditBirthDateCommand> logger)
    : ICommandHandler
{
    public BotCommand Command => new() { Command = "ebd", Description = "Обновить дату рождения (формат: 10.08.2001 можно без года))" };
    public UserType CommandUserType => UserType.Default;

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        var chat = update.Message!.Chat;
        var messageText = update.Message!.Text;
        var user = await userRepository.GetByAsync(u =>
                       u.TelegramUserId == update.Message.From.Id ||
                       u.TelegramTag == update.Message.From.Username) ??
                   new ChatUser()
                   {
                       TelegramUserId = update.Message.From.Id,
                       TelegramTag = update.Message.From.Username,
                       RegisterDate = DateTime.Now,
                   };
        var date = Regex.Match(
            messageText,
            @"\/ebd(?:@\S+)?\s+(\d{2}\.\d{2}(\.\d{4})?)").Groups[1].Value;
        var isBirthDateValid = BirthDateHelper.TryParseDateWithDefaultYear(
            date,
            1000,
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
                text: "Дата не корректна, формат даты - 10.08.2001 (можно без года)"
            );
            logger.LogWarning($"User " +
                              $"@{user.GetTag()}\n" +
                              $"can not update birth date to {date}");
        }
    }
}
