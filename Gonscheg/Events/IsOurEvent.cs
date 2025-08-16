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
                    PhotoPath = u.CarPhotoPath,
                    Plate = u.Plate,
                    Tag = u.GetTag(),
                    Ratio = FuzzySharp.Levenshtein.GetRatio(plate.Unidecode().ToUpper(), u.Plate.Unidecode().ToUpper())
                })
                .Where(r => r.Ratio >= 0.6)
                .OrderByDescending(r => r.Ratio)
                .ToList();

            var carPhotosPath = "/app/car_photos";

            switch (bestMatches.Count)
            {
                case > 0 when bestMatches.First().Ratio == 1:
                {
                    if (!string.IsNullOrEmpty(bestMatches.First().PhotoPath))
                    {
                        await using var fileStream = File.OpenRead(bestMatches.First().PhotoPath);
                        var photo = new InputFileStream(fileStream);
                        await botClient.SendPhoto(
                            chatId: chatId,
                            photo: photo,
                            caption: $"Ага, {bestMatches.First().Tag}",
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await botClient.SendMessage(
                            chatId: chatId,
                            text: $"Ага, {bestMatches.First().Tag}",
                            cancellationToken: cancellationToken);
                    }

                    break;
                }
                case 1:
                    if (!string.IsNullOrEmpty(bestMatches.First().PhotoPath))
                    {
                        await using var fileStream = File.OpenRead(bestMatches.First().PhotoPath);
                        var photo = new InputFileStream(fileStream);
                        await botClient.SendPhoto(
                            chatId: chatId,
                            photo: photo,
                            caption: $"Думаю да, скорее всего {bestMatches.First().Tag}\n" +
                                     $"Но это не точно",
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await botClient.SendMessage(
                            chatId: chatId,
                            text: $"Думаю да, скорее всего {bestMatches.First().Tag}\n" +
                                  $"Но это не точно",
                            cancellationToken: cancellationToken);
                    }
                    break;
                case > 1:
                    var photos = new List<InputMediaPhoto>();
                    var streams = new List<FileStream>();
                    foreach (var match in bestMatches.Skip(1))
                    {
                        var fileStream = File.OpenRead(match.PhotoPath);
                        streams.Add(fileStream);
                        photos.Add(new InputMediaPhoto(new InputFileStream(fileStream)));
                    }

                    if (photos.Any())
                    {
                        photos[0].Caption = $"Хуй знает... " +
                                            $"Вероятнее всего {bestMatches.First().Tag}\n" +
                                            $"Но возможно это кто-то из них... {string.Join("\n", bestMatches.Skip(1).Select(bm => bm.Tag))}";
                        await botClient.SendMediaGroup(
                            chatId: chatId,
                            media: photos,
                            cancellationToken: cancellationToken);
                        foreach (var stream in streams)
                        {
                            await stream.DisposeAsync();
                        }
                    }
                    else
                    {
                        await botClient.SendMessage(
                            chatId: chatId,
                            text: $"Хуй знает... " +
                                  $"Вероятнее всего {bestMatches.First().Tag}\n" +
                                  $"Но возможно это кто-то из них... {string.Join("\n", bestMatches.Skip(1).Select(bm => bm.Tag))}",
                            cancellationToken: cancellationToken);
                    }
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
