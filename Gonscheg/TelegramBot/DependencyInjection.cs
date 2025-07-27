using Gonscheg.Commands;
using Gonscheg.Extensions;
using Gonscheg.Handlers;
using Gonscheg.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Gonscheg.TelegramBot;

public static class DependencyInjection
{
    public static IServiceCollection AddTelegramBot(this IServiceCollection services)
    {
        services.AddSingleton<BotClient>();
        services.AddScoped<MorningExtension>();
        services.AddScoped<ShodkaExtension>();
        services.AddScoped<StartCommand>();
        services.AddScoped<TestCommand>();
        services.AddScoped<WeatherCommand>();
        services.AddScoped<UpdateHandler>();
        services.AddScoped<ErrorHandler>();
        services.AddSingleton<WeatherClient>();
        services.AddHostedService<BotHostedService>();
        return services;
    }
}