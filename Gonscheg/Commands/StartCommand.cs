using Gonscheg.Application.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Chat = Gonscheg.Domain.Entities.Chat;

namespace Gonscheg.Commands;

public class StartCommand(IBaseCRUDRepository<Chat> chatRepository, ILogger<StartCommand> logger)
{
    public static readonly BotCommand Command = new() { Command = "start", Description = "Start the bot" };

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        if (update is { Type: UpdateType.Message, Message.Text: "/start@gonscheg_nelegalniy_bot" })
        {
            var chat = update.Message.Chat;
            if (await chatRepository.GetByAsync(c => c.ChatId == chat.Id.ToString()) == null)
            {
                await chatRepository.CreateAsync(new Chat(chat.Id.ToString(), update.Message.Chat.Title));
                await botClient.SendMessage(
                    chatId: chat.Id,
                    text: "Ну дарова потужнi хондаводи"
                );

                logger.LogInformation($"Chat {chat.Id} was added");
            }
            else
            {
                await botClient.SendMessage(
                    chatId: chat.Id,
                    text: "Та вы уже добавлены, тиха"
                );

                logger.LogWarning($"Chat {chat.Id} was added before");
            }
        }
    }
}