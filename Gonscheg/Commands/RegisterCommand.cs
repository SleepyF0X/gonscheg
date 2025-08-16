using System.Text.RegularExpressions;
using Gonscheg.Application.Repositories;
using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Entities;
using Gonscheg.Domain.Enums;
using Gonscheg.Helpers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Unidecode.NET;

namespace Gonscheg.Commands;

public class RegisterCommand(
    IBaseCRUDRepository<ChatUser> userRepository,
    ILogger<RegisterCommand> logger)
    : ICommandHandler
{
    public BotCommand Command => new() { Command = "reg", Description = "Register" };
    public UserType CommandUserType => UserType.Default;

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        var messageText = update.Message?.Text ?? update.Message?.Caption;
        var chat = update.Message.Chat;
        var user = await userRepository.GetByAsync(u =>
                       u.TelegramUserId == update.Message.From.Id ||
                       (u.TelegramTag!= null && u.TelegramTag == update.Message.From.Username)) ??
                   new ChatUser
                   {
                       TelegramUserId = update.Message.From.Id,
                       TelegramTag = update.Message.From.Username,
                       RegisterDate = DateTime.Now,
                   };
        var match = Regex.Match(
            messageText,
            @"^\/reg\s*((1\.\s*(.+)\s*)|())((2\.\s*(.+)\s*)|())((3\.\s*(.*)\s*)|())((4\.\s*(.*))|())",
            RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            await botClient.SendMessage(
                chatId: chat.Id,
                text: TelegramMarkdownV2Helper.Escape("Сообщение о регистрации не распознано" +
                                                      "Проверьте формат сообщения, параметры должны быть в указанном порядке\n" +
                                                      ">/reg\n" +
                                                      ">1. Имя (или позывной)\n" +
                                                      ">2. Гос номер\n" +
                                                      ">3. Дата рождения (в формате 10.08.2001, можно без года)\n" +
                                                      ">4. VIN код (дли истории машины, прошлых владельцев искать)", '>')
            );

            return;
        }

        var name = match.Groups[3].Value.TrimStart().TrimEnd();
        var plate = match.Groups[7].Value.Unidecode().ToUpper().Trim();
        var isBirthDateValid = BirthDateHelper.TryParseDateWithDefaultYear(match.Groups[11].Value.Trim(), 1000, out var birthdate);
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

        if (!string.IsNullOrWhiteSpace(vinCode) && vinCode.Length is < 16 or > 18)
        {
            user.VinCode = !string.IsNullOrEmpty(vinCode) ? vinCode : user.VinCode;
        }

        user.Name = name;

        if (!errors.Any())
        {
            if (update.Message?.Photo is not null && update.Message.Photo.Length > 0)
            {
                var photo = update.Message.Photo.Last();

                var file = await botClient.GetFileAsync(photo.FileId);
                var filePath = file.FilePath!;

                var directoryPath = "/app/car_photos";
                var fileName = $"{user.TelegramUserId}.jpg";
                var fullPath = Path.Combine(directoryPath, fileName);

                Directory.CreateDirectory(directoryPath);

                user.CarPhotoPath = fullPath;

                await using var fileStream = new FileStream(fullPath, FileMode.Create);
                await botClient.DownloadFile(filePath, fileStream);
            }

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
                                  $"{user.GetTag()}\n" +
                                  $"{user.Name}\n" +
                                  $"{user.Plate}\n" +
                                  $"{user.VinCode}\n" +
                                  $"{user.BirthDate}\n" +
                                  $"successfully registered");
        }
        else
        {
            await botClient.SendMessage(
                chatId: chat.Id,
                text: TelegramMarkdownV2Helper.Escape("Регистрация не удалась\n" +
                                                      $"Ошибки:\n {string.Join("\n", errors)}\n" +
                                                      "Проверьте формат сообщения, параметры должны быть в указанном порядке\n" +
                                                      ">/reg\n" +
                                                      ">1. Имя (или позывной)\n" +
                                                      ">2. Гос номер\n" +
                                                      ">3. Дата рождения (в формате 10.08.2001, можно без года)\n" +
                                                      ">4. VIN код (дли истории машины, прошлых владельцев искать)", '>'),
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
