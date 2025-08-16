using Gonscheg.Application.Repositories;
using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Domain.Entities;
using Gonscheg.TelegramBot;
using Microsoft.Extensions.Logging;
using Quartz;
using Telegram.Bot;

namespace Gonscheg.Jobs;

/// <summary>
/// Job for sending a message every Friday at 12:00 Kyiv time.
/// </summary>
[DisallowConcurrentExecution] // Ensures the job won't run in parallel if the previous execution is still in progress
public class ShodkaJob : IQuartzJob
{
    public static string CRONTime => "0 0 12 ? * FRI";
    private readonly IBaseCRUDRepository<Chat> _chatRepository;
    private readonly BotClient _botClient;
    private readonly ILogger<ShodkaJob> _logger;

    public ShodkaJob(
        IBaseCRUDRepository<Chat> chatRepository,
        BotClient botClient,
        ILogger<ShodkaJob> logger)
    {
        _chatRepository = chatRepository;
        _botClient = botClient;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var chats = await _chatRepository.GetAllAsync();

        foreach (var chat in chats)
        {
            await _botClient.Client.SendMessage(
                chatId: chat.ChatId,
                text: "Миллионы приходят, уходят, не в них счастье.\n" +
                      "Самым важным на свете всегда будут люди в этой комнате, вот здесь, сейчас.\n" +
                      "Осталось договориться о времени."
            );

            _logger.LogInformation(
                "SHODKA message to chat {ChatName} (id: {ChatId}) successfully sent",
                chat.Name,
                chat.ChatId
            );
        }
    }
}
