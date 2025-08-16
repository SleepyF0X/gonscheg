using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Gonscheg.Commands;

public class TestCommand
    : ICommandHandler
{
    public BotCommand Command => new() { Command = "test", Description = "Test" };
    public UserType CommandUserType => UserType.Admin;

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Update update)
    {
        var chatId = update.Message.Chat.Id;
        var chatAdmins = await botClient.GetChatAdministrators(chatId);
        if (chatAdmins.Any(admin => admin.User.Id == update.Message.From?.Id))
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "Ну тестируй, хуйлуша!"
            );
        }
    }
}
