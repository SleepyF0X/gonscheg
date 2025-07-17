using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using Gonscheg.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Unidecode.NET;

namespace Gonscheg.Commands;

public class IsOurEvent
{
    private static string _spreadsheetId = "1DuMqXvw8efCE3xzH6dcErYa5Z0JUk65x";
    private static string _gid = "1131984113";

    public static async Task HandleEvent(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;
        var plate = PlateExtractor.TryExtractPlateFromMessage(update.Message.Text, out var plateIsValid);
        if (update.Type is UpdateType.Message &&
            update.Message?.Text != null &&
            plateIsValid)
        {
            var csvUrl = $"https://docs.google.com/spreadsheets/d/{_spreadsheetId}/export?format=csv&gid={_gid}";

            var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(csvUrl, cancellationToken);

            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var rows = new List<string[]>();
            while (await csv.ReadAsync())
            {
                var row = csv.Parser.Record;
                rows.Add(row);
            }

            var numbers = new Dictionary<string, string>();
            for (var i = 2; i < rows.Count; i++)
            {
                var row = rows[i];
                if (row.Length >= 2)
                {
                    var key = row[1].Trim().Unidecode();
                    var value = row[0];
                    if (PlateExtractor.IsPlateValid(key))
                    {
                        numbers[key] = value;
                    }
                }
            }

            var bestMatches = numbers
                .Select(no => new
                {
                    Number = no.Key,
                    Owner = no.Value,
                    Ratio = FuzzySharp.Levenshtein.GetRatio(plate?.Unidecode().ToUpper(), no.Key.Unidecode().ToUpper())
                })
                .Where(r => r.Ratio >= 0.6)
                .OrderByDescending(r => r.Ratio)
                .ToList();

            switch (bestMatches.Count)
            {
                case > 0 when bestMatches.First().Ratio == 1:
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: !string.IsNullOrWhiteSpace(bestMatches.First().Owner)
                            ? $"Да, @{bestMatches.First().Owner}"
                            : "Да, но тега нет, анонимус блять",
                        cancellationToken: cancellationToken);
                    break;
                case 1:
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: !string.IsNullOrWhiteSpace(bestMatches.First().Owner)
                            ? $"Думаю да, скорее всего @{bestMatches.First().Owner}"
                            : "Думаю да, но тега нет, анонимус блять",
                        cancellationToken: cancellationToken);
                    break;
                case > 1:
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: $"Хуй знает... " +
                              $"Вероятнее всего {(!string.IsNullOrWhiteSpace(bestMatches.First().Owner) ? $"@{bestMatches.First().Owner}" :"(тега нет)")}" +
                              "\n" +
                              $"Но возможно это кто-то из них... {string.Join(", ", bestMatches.Skip(1).Select(bm => !string.IsNullOrWhiteSpace(bm.Owner) ? $"@{bm.Owner}" : "аноним"))}",
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

        if (!plateIsValid && plate != null)
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "Не могу понять, точно номер правильно ввел? Ну или ты криворукий",
                cancellationToken: cancellationToken);
        }
    }
}