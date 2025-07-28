using Gonscheg.Application.Repositories;
using Gonscheg.Domain;
using Gonscheg.Domain.Entities;
using Gonscheg.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Unidecode.NET;

namespace Gonscheg.Events;

public class IsOurEvent(IBaseCRUDRepository<ChatUser> userRepository)
{
    public async Task HandleEventAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(update.Message?.Text))
        {
            return;
        }

        var chatId = update.Message.Chat.Id;
        var plate = PlateExtractor.ExtractPlateFromNashessage(update.Message.Text);
        if (string.IsNullOrWhiteSpace(plate))
        {
            return;
        }

        var plateIsValid = PlateExtractor.IsPlateValid(plate);
        if (update.Type is UpdateType.Message &&
            update.Message?.Text != null &&
            plateIsValid)
        {
            var users = await userRepository.GetAllAsync();
            var bestMatches = users
                .Select(u => new
                {
                    Plate = u.Plate,
                    TelegramTag = u.TelegramTag,
                })
                .ToArray()
                .Select(u => new
                {
                    Plate = u.Plate,
                    TelegramTag = u.TelegramTag,
                    Ratio = FuzzySharp.Levenshtein.GetRatio(plate.Unidecode().ToUpper(), u.Plate.Unidecode().ToUpper())
                })
                .Where(r => r.Ratio >= 0.6)
                .OrderByDescending(r => r.Ratio)
                .ToList();

            switch (bestMatches.Count)
            {
                case > 0 when bestMatches.First().Ratio == 1:
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: !string.IsNullOrWhiteSpace(bestMatches.First().TelegramTag)
                            ? $"Да, @{bestMatches.First().TelegramTag}"
                            : "Да, но тега нет, анонимус блять",
                        cancellationToken: cancellationToken);
                    break;
                case 1:
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: !string.IsNullOrWhiteSpace(bestMatches.First().TelegramTag)
                            ? $"Думаю да, скорее всего @{bestMatches.First().TelegramTag}"
                            : "Думаю да, но тега нет, анонимус блять",
                        cancellationToken: cancellationToken);
                    break;
                case > 1:
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: $"Хуй знает... " +
                              $"Вероятнее всего {(!string.IsNullOrWhiteSpace(bestMatches.First().TelegramTag) ? $"@{bestMatches.First().TelegramTag}" : "(тега нет)")}" +
                              "\n" +
                              $"Но возможно это кто-то из них... {string.Join(", ", bestMatches.Skip(1).Select(bm => !string.IsNullOrWhiteSpace(bm.TelegramTag) ? $"@{bm.TelegramTag}" : "аноним"))}",
                        cancellationToken: cancellationToken);
                    break;
                default:
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: "Не ебу, скорее всего нет, спросите Хромова, может он знает",
                        cancellationToken: cancellationToken);
                    break;
            }
        }

        if (!plateIsValid)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "Не могу понять, точно номер правильно ввел? Ну или ты криворукий",
                cancellationToken: cancellationToken);
        }
    }
}