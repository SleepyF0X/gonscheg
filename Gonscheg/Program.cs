using Gonscheg.Infrastructure;
using Gonscheg.Shared;
using Gonscheg.TelegramBot;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

try
{
    Log.Information("Starting up...");

    EnvironmentVariables.InitializeEnvironmentVariables();

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((ctx, services) =>
        {
            services.AddInfrastructure();

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