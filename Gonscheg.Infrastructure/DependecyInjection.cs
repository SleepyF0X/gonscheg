using Gonscheg.Application.Repositories;
using Gonscheg.Infrastructure.Persistence;
using Gonscheg.Infrastructure.Repositories;
using Gonscheg.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gonscheg.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(Constants.ConnectionString);
        });

        services.AddScoped(typeof(IBaseCRUDRepository<>), typeof(BaseCRUDRepository<>));

        return services;
    }

    public static async Task UpdateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataContext>>();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        try
        {
            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToArray();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Pending migrations found: {Count}", pendingMigrations.Count());
                foreach (var migration in pendingMigrations)
                {
                    logger.LogInformation("Pending migration: {Migration}", migration);
                }

                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date. No migrations needed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }
}