using Gonscheg.Application.Repositories;
using Gonscheg.Domain;
using Gonscheg.Domain.Entities;
using Microsoft.Extensions.Logging;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.Events;

public class NewMemberEvent(IBaseCRUDRepository<ChatUser> userRepository, ILogger<NewMemberEvent> logger)
{
    public async Task HandleEventAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update is { Type: UpdateType.Message, Message.Type: MessageType.NewChatMembers })
        {
            var newMembers = update.Message.NewChatMembers;

            if (newMembers != null)
            {
                foreach (var newMember in newMembers)
                {
                    if (newMember.IsBot) continue;

                    var chatId = update.Message.Chat.Id;
                    var userName = !string.IsNullOrEmpty(newMember.Username) ? $"@{newMember.Username}" : $"{newMember.FirstName} {newMember.LastName}";

                    await botClient.SendMessage(
                        chatId,
                        text: $"Привет, {userName} \nДавно тебя не было в Уличных Гонках! Показывай аппарат!",
                        cancellationToken: cancellationToken);
                    await botClient.SendMessage(
                        chatId,
                        text: "И зарегистрируйся пожалуйста\\.\n" +
                              "Команда:\n" +
                              "> \\/reg\n" +
                              "> 1\\. Имя\n" +
                              "> 2\\. Гос номер\n" +
                              "> 3\\. Дата рождения \\(в формате 10\\.08\\.2001\\)\n" +
                              "> 4\\. VIN код",
                        parseMode: ParseMode.MarkdownV2,
                        cancellationToken: cancellationToken);
                }
            }
        }
    }
}