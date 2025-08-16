using Gonscheg.Application.Repositories;
using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Entities;
using Gonscheg.Helpers;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.Events;

public class NewMemberEvent(
    IBaseCRUDRepository<ChatUser> userRepository,
    ILogger<NewMemberEvent> logger)
    : IEventHandler
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
                    var userName = !string.IsNullOrEmpty(newMember.Username)
                        ? $"@{newMember.Username}"
                        : $"[{newMember.FirstName} {newMember.LastName}](tg://user?id={newMember.Id})";

                    var user = await userRepository.GetByAsync(u =>
                        u.TelegramUserId == update.Message.From.Id ||
                        u.TelegramTag == update.Message.From.Username);

                    if (user == null)
                    {
                        await botClient.SendMessage(
                            chatId,
                            text: $"Привет, {userName} \nДавно тебя не было в Уличных Гонках\\! Показывай аппарат\\!",
                            parseMode: ParseMode.MarkdownV2,
                            cancellationToken: cancellationToken);
                        await botClient.SendMessage(
                            chatId,
                            text: TelegramMarkdownV2Helper.Escape("И зарегистрируйся пожалуйста\n" +
                                                                  "Команда:\n" +
                                                                  ">/reg\n" +
                                                                  ">1. Имя (или позывной)\n" +
                                                                  ">2. Гос номер\n" +
                                                                  ">3. Дата рождения (в формате 10.08.2001, можно без года)\n" +
                                                                  ">4. VIN код (дли истории машины, прошлых владельцев искать)\n" +
                                                                  "и прикрепи одно фото машины", '>'),
                            parseMode: ParseMode.MarkdownV2,
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await botClient.SendMessage(
                            chatId,
                            text: $"Привет, {userName} \nДавно тебя не было в Уличных Гонках\\!\nА чего уходил то?\\!",
                            parseMode: ParseMode.MarkdownV2,
                            cancellationToken: cancellationToken);
                    }


                    logger.LogInformation($"New member {userName}");
                }
            }
        }
    }
}
