using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Gonscheg.Events;

public class NewMemberEvent
{
    public static async Task HandleEventAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
                        text: $"Привет, {userName} \nДавно тебя не было в Уличных Гонках!",
                        cancellationToken: cancellationToken);
                }
            }
        }
    }
}