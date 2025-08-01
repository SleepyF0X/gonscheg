using System.Text.RegularExpressions;
using Gonscheg.Application.Repositories;
using Gonscheg.Domain;
using Gonscheg.Domain.Entities;
using Gonscheg.Helpers;
using Microsoft.Extensions.Logging;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Unidecode.NET;

namespace Gonscheg.Commands;

public class RegisterCommand(IBaseCRUDRepository<ChatUser> userRepository, ILogger<RegisterCommand> logger)
{
    public static readonly BotCommand Command = new() { Command = "reg", Description = "Register" };

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        if (update.Type == UpdateType.Message &&
            (update.Message?.Text?.Contains("/reg") ?? false))
        {
            var messageText = update.Message!.Text;
            var chat = update.Message.Chat;
            var user = await userRepository.GetByAsync(u =>
                           u.TelegramUserId == update.Message.From.Id ||
                           u.TelegramTag == update.Message.From.Username) ??
                       new ChatUser()
                       {
                           TelegramUserId = update.Message.From.Id,
                           TelegramTag = update.Message.From.Username,
                       };
            var match = Regex.Match(
                messageText,
                @"^\/reg\s*((1\.\s*(.+)\s*)|())((2\.\s*(.+)\s*)|())((3\.\s*(.*)\s*)|())((4\.\s*(.*))|())",
                RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                await botClient.SendMessage(
                    chatId: chat.Id,
                    text: $"Сообщение о регистрации не распознано"
                );

                return;
            }

            var name = match.Groups[3].Value.TrimStart().TrimEnd();
            var plate = match.Groups[7].Value.Unidecode().ToUpper().Trim();
            var isBirthDateValid = DateTime.TryParse(match.Groups[11].Value.Trim(), out var birthdate);
            var vinCode = !string.IsNullOrEmpty(match.Groups[15].Value.Trim())
                ? match.Groups[15].Value.Trim().ToUpper()
                : null;

            var errors = new List<string>();

            if (PlateExtractor.IsPlateValid(plate))
            {
                user.Plate = plate;
            }
            else
            {
                errors.Add("номер не валидный");
            }

            if (isBirthDateValid)
            {
                user.BirthDate = birthdate;
            }
            else
            {
                errors.Add("дата рождения не валидна");
            }

            user.VinCode = !string.IsNullOrEmpty(vinCode?.ToUpper()) ? vinCode.ToUpper() : user.VinCode;
            user.Name = name;

            if (!errors.Any())
            {
                if (user.Id != 0)
                {
                    await userRepository.UpdateAsync(user);
                    await botClient.SendMessage(
                        chatId: chat.Id,
                        text: "Данные обновлены"
                    );
                }
                else
                {
                    await userRepository.CreateAsync(user);
                    await botClient.SendMessage(
                        chatId: chat.Id,
                        text: "Зарегистрирован успешно"
                    );
                }

                logger.LogInformation($"User " +
                                      $"@{user.TelegramTag}\n" +
                                      $"{user.Plate}\n" +
                                      $"{user.VinCode}\n" +
                                      $"{user.BirthDate}\n" +
                                      $"successfully registered");
            }
            else
            {
                await botClient.SendMessage(
                    chatId: chat.Id,
                    text: "Регистрация не удалась\n" +
                          $"Ошибки:\n {string.Join("\n", errors)}" +
                          "Проверьте формат сообщения, параметры должны быть в указанном порядке\n" +
                          "> \\/reg\n" +
                          "> 1\\. Имя\n" +
                          "> 2\\. Гос номер\n" +
                          "> 3\\. Дата рождения \\(в формате 10\\.08\\.2001\\)\n" +
                          "> 4\\. VIN код",
                    parseMode: ParseMode.MarkdownV2
                );

                logger.LogInformation($"User " +
                                      $"@{user.TelegramTag}\n" +
                                      $"{user.Plate}\n" +
                                      $"{user.VinCode}\n" +
                                      $"{user.BirthDate}\n" +
                                      $"not registered errors: {string.Join(", ", errors)}");
            }
        }
    }
}