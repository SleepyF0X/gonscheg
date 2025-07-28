using Gonscheg.Commands;
using Gonscheg.Events;
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

        AddExtensions(services);
        AddCommands(services);
        AddHandlers(services);
        AddEvents(services);

        services.AddHostedService<BotHostedService>();
        return services;
    }

    // Extensions for hosted services (for example send message every day at 7 AM)
    private static void AddExtensions(IServiceCollection services)
    {
        services.AddSingleton<MorningExtension>();
        services.AddSingleton<ShodkaExtension>();
    }

    // Bot commands which users can write
    private static void AddCommands(IServiceCollection services)
    {
        services.AddScoped<StartCommand>();
        services.AddScoped<TestCommand>();
        services.AddScoped<WeatherCommand>();
        services.AddScoped<RegisterCommand>();
        services.AddScoped<EditBirthDateCommand>();
    }

    // Default Telegram handlers
    private static void AddHandlers(IServiceCollection services)
    {
        services.AddTransient<UpdateHandler>();
        services.AddTransient<ErrorHandler>();
    }

    //Events (for example: new user added)
    private static void AddEvents(IServiceCollection services)
    {
        services.AddScoped<IsOurEvent>();
        services.AddScoped<NewMemberEvent>();
    }
}