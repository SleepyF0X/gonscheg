using Gonscheg.Helpers;
using Gonscheg.Infrastructure;
using Gonscheg.Shared;
using Gonscheg.Shared.Shared;
using Gonscheg.TelegramBot;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using TelegramSink;

try
{
    Log.Information("Starting up...");

    EnvironmentVariables.InitializeEnvironmentVariables();

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.Logger(lc => lc
            .Filter.ByExcluding(le =>
                le.Properties.TryGetValue("SourceContext", out var source) &&
                source.ToString().Contains("Microsoft.EntityFrameworkCore"))
            .WriteTo.TeleSink(
                EnvironmentVariables.TelegramBotToken,
                EnvironmentVariables.TelegramLogChatId,
                minimumLevel: LogEventLevel.Information
            )
        )
        .Enrich.FromLogContext()
        .MinimumLevel.Information()
        .CreateLogger();

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((ctx, services) =>
        {
            services.AddInfrastructure();

            services.AddSingleton<WeatherClient>();
            services.AddLogging();
            services.AddTelegramBot();
        })
        .Build();

    await host.UpdateDatabase();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}