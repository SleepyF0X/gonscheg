using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Gonscheg.Handlers;

public class ErrorHandler(ILogger<ErrorHandler> logger)
{
    public Task Handle(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var errorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        logger.LogError(errorMessage);
        return Task.CompletedTask;
    }
}
