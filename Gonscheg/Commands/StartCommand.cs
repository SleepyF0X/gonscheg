using Gonscheg.Application.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Chat = Gonscheg.Domain.Chat;

namespace Gonscheg.Commands;

public class StartCommand(IBaseCRUDRepository<Chat> chatRepository, ILogger<StartCommand> logger)
{
    public static readonly BotCommand Command = new() { Command = "start", Description = "Запустить бота" };

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        if (update is { Type: UpdateType.Message, Message.Text: "/start" or "/start@gonscheg_nelegalniy_bot" })
        {
            var chatId = update.Message.Chat.Id;
            if (await chatRepository.GetByAsync(c => c.ChatId == chatId.ToString()) == null)
            {
                await chatRepository.CreateAsync(new Chat(chatId.ToString()));
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Ну дарова потужнi хондаводи"
                );
                logger.LogInformation($"Чат {chatId} был добавлен");
            }
            else
            {
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Та вы уже добавлены, тиха"
                );
            }
        }
    }
}