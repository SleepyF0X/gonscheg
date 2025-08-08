using Gonscheg.Extensions;
using Gonscheg.Handlers;
using Microsoft.Extensions.Hosting;

namespace Gonscheg.TelegramBot;

public class BotHostedService(
    BotClient botClient,
    UpdateHandler updateHandler,
    ErrorHandler errorHandler)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await botClient.StartClientAsync(updateHandler.Handle, errorHandler.Handle);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
