using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.Commands;

public class StartCommand
{
    public static BotCommand Command = new() { Command = "start", Description = "Запустить бота" };

    public static async Task HandleCommand(ITelegramBotClient botClient, Update update, HashSet<long> chatIds)
    {
        if (update is { Type: UpdateType.Message, Message: not null } && update.Message.Text is "/start" or "/start@gonscheg_nelegalniy_bot")
        {
            var chatId = update.Message.Chat.Id;
            await botClient.SendMessage(
                chatId: chatId,
                text: "Ну дарова потужнi хондаводи"
            );
            Console.WriteLine($"Чат {chatId} был добавлен");
            chatIds.Add(chatId);
        }
    }
}