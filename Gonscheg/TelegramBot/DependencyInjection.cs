using System.Reflection;
using Gonscheg.Application.TelegramBotInterfaces;
using Gonscheg.Dispatchers;
using Gonscheg.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Gonscheg.TelegramBot;

public static class DependencyInjection
{
    public static IServiceCollection AddTelegramBot(this IServiceCollection services)
    {
        services.AddSingleton<BotClient>();
        AddJobs(services);
        AddCommands(services);
        AddHandlers(services);
        AddEvents(services);

        services.AddHostedService<BotHostedService>();
        return services;
    }

    // Jobs (for example send message every day at 7 AM)
    private static void AddJobs(IServiceCollection services)
    {
        var iJobType = typeof(IQuartzJob);
        services.AddQuartz(q =>
        {
            var jobTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => iJobType.IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
                .ToList();

            foreach (var jobType in jobTypes)
            {
                var jobKey = JobKey.Create(jobType.Name);
                var CRONTime =jobType.GetProperty(
                    nameof(IQuartzJob.CRONTime),
                    BindingFlags.Public | BindingFlags.Static)?.GetValue(null)?.ToString();
                q.AddJob(jobType, jobKey);
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{jobKey}Trigger")
                    .WithCronSchedule(CRONTime, x => x
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv"))
                    )
                );
            }
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }

    // Bot commands which users can write
    private static void AddCommands(IServiceCollection services)
    {
        services.AddScoped<CommandDispatcher>();

        var commandHandlerType = typeof(ICommandHandler);

        var commandHandlers = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => commandHandlerType.IsAssignableFrom(p) && p is { IsInterface: false, IsAbstract: false });

        foreach (var commandHandler in commandHandlers)
        {
            services.AddScoped(commandHandler);
            services.AddScoped(commandHandlerType, commandHandler);
        }
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
        services.AddScoped<EventDispatcher>();
        var eventHandlerType = typeof(IEventHandler);

        var eventHandlers = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => eventHandlerType.IsAssignableFrom(p) && p is { IsInterface: false, IsAbstract: false });

        foreach (var eventHandler in eventHandlers)
        {
            services.AddScoped(eventHandler);
            services.AddScoped(eventHandlerType, eventHandler);
        }
    }
}
