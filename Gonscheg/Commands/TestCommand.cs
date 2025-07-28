using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.Commands;

public class TestCommand()
{
    public static readonly BotCommand Command = new() { Command = "test", Description = "Test" };

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        if (update is { Type: UpdateType.Message, Message.Text: "/test" or "/test@gonscheg_nelegalniy_bot" })
        {
            var chatId = update.Message.Chat.Id;
            var chatAdmins = await botClient.GetChatAdministrators(chatId);
            var chat = await botClient.GetChat("-2163831637");
            if (chatAdmins.Any(admin => admin.User.Id == update.Message.From?.Id))
            {
                await botClient.SendMessage(
                    chatId: chatId,
                    text: "Ну тестируй, хуйлуша"
                );
            }
        }
    }
}