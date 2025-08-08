using System.Text;
using Gonscheg.Application.Repositories;
using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Entities;
using Gonscheg.Helpers;
using Gonscheg.TelegramBot;
using Microsoft.Extensions.Logging;
using Quartz;
using Telegram.Bot;

namespace Gonscheg.Extensions;

public class MorningJob(
    BotClient botClient,
    WeatherClient weatherClient,
    IBaseCRUDRepository<Chat> chatRepository,
    IBaseCRUDRepository<ChatUser> userRepository,
    ILogger<MorningJob> logger)
    : IQuartzJob
{
    public static string CRONTime => "0 0 8 * * ?";

    public async Task Execute(IJobExecutionContext context)
    {
        foreach (var chat in await chatRepository.GetAllAsync())
        {
            await botClient.Client.SendTextMessageAsync(
                chatId: chat.ChatId,
                text: "Ранок потужнi хондаводи!"
            );
            await botClient.Client.SendTextMessageAsync(
                chatId: chat.ChatId,
                text: await weatherClient.GeDayWeatherMessageAsync("Europe/Kyiv")
            );

            var birthdayUsers = await userRepository
                .GetAllByAsync(u =>
                    new DateOnly(1, u.BirthDate.Value.Month, u.BirthDate.Value.Day) ==
                    new DateOnly(1, DateTime.Now.Month, DateTime.Now.Day));

            if (birthdayUsers.Any())
            {
                var sb = new StringBuilder("А вы знали что сегодня день рождения у...");
                foreach (var birthdayUser in birthdayUsers)
                {
                    sb.AppendLine($"@{birthdayUser.GetTag()}");
                }

                await botClient.Client.SendTextMessageAsync(
                    chatId: chat.ChatId,
                    text: "Ранок потужнi хондаводи!"
                );
            }

            logger.LogInformation($"MORNING message to chat\n" +
                                  $"{chat.Name}\n" +
                                  $"with id: {chat.ChatId}\n" +
                                  $"SUCCESSFULLY SENT");
        }
    }
}
