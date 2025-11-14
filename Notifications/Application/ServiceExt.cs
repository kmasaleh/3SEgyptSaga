using MassTransit;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Notifications.Application.Sagas;
using Notifications.Persistence;

namespace Notifications.Application;

public static class ServiceExt
{
    internal class EntityFrameworkSagaRepositoryProviderSetup : ISagaRepositoryRegistrationProvider
    {
        void ISagaRepositoryRegistrationProvider.Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
        {
            configurator.Repository(cfg =>
            {
                cfg.AddDbContext<DbContext, NotificationsDbContext>((provider, optionsBuilder) =>
                {
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    optionsBuilder.EnableSensitiveDataLogging();
                });
            });

        }

    }

    static public Task InstallServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<NotificationsDbContext>
        (
            (serviceProvider, options) => options.UseSqlServer(connectionString)
        );
        services.AddScoped<INotificationsService, NotificationsService>();
        return Task.CompletedTask;
    }
    public static async Task ApplyMigrationAsync(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        try
        {

            var db = scope.ServiceProvider.GetService<NotificationsDbContext>();
            var con = db.Database.GetConnectionString();
            logger!.LogInformation($"db connection string {con}");
            await db!.Database.MigrateAsync();
            logger!.LogInformation("Notifications migration applied successfully.");
        }
        catch (Exception ex)
        {
            logger!.LogError("An error occurred while applying Notifications migration .");
            logger!.LogError(ex.Message);
            throw;
        }

    }
}
