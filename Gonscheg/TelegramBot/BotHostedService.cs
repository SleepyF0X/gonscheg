using Gonscheg.Extensions;
using Gonscheg.Handlers;
using Microsoft.Extensions.Hosting;

namespace Gonscheg.TelegramBot;

public class BotHostedService(
    BotClient botClient,
    UpdateHandler updateHandler,
    ErrorHandler errorHandler,
    MorningExtension morningExtension,
    ShodkaExtension shodkaExtension)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await botClient.StartClientAsync(updateHandler.Handle, errorHandler.Handle);
        morningExtension.StartExtension();
        shodkaExtension.StartExtension();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}