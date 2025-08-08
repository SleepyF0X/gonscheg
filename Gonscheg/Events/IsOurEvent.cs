using Gonscheg.Application.Repositories;
using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Entities;
using Gonscheg.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Unidecode.NET;

namespace Gonscheg.Events;

public class IsOurEvent(IBaseCRUDRepository<ChatUser> userRepository) : IEventHandler
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
                .ToArray()
                .Select(u => new
                {
                    Plate = u.Plate,
                    Tag = u.GetTag(),
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
                        text: $"Ага, {bestMatches.First().Tag}",
                        cancellationToken: cancellationToken);
                    break;
                case 1:
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: $"Думаю да, скорее всего {bestMatches.First().Tag}\n" +
                              $"Но это не точно",
                        cancellationToken: cancellationToken);
                    break;
                case > 1:
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: $"Хуй знает... " +
                              $"Вероятнее всего {bestMatches.First().Tag}\n" +
                              $"Но возможно это кто-то из них... {string.Join("\n", bestMatches.Skip(1).Select(bm => bm.Tag))}",
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
                text: "Не могу понять, точно номер правильно ввел?",
                cancellationToken: cancellationToken);
        }
    }
}
