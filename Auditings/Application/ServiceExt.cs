using MassTransit;
using MassTransit.Configuration;
using Microsoft.EntityFrameworkCore;
using Auditings.Persistence;

namespace Auditings.Application;

public static class ServiceExt
{
    

    static public Task InstallServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AuditingDbContext>
        (
            (serviceProvider, options) => options.UseSqlServer(connectionString)
        );

        services.AddSingleton< FailureService >();
        return Task.CompletedTask;
    }
    public static async Task ApplyMigrationAsync(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        try
        {

            var db = scope.ServiceProvider.GetService<AuditingDbContext>();
            var con = db.Database.GetConnectionString();
            logger!.LogInformation($"db connection string {con}");
            await db!.Database.MigrateAsync();
            logger!.LogInformation("Email migration applied successfully.");
        }
        catch (Exception ex)
        {
            logger!.LogError("An error occurred while applying Emails migration .");
            logger!.LogError(ex.Message);
            throw;
        }

    }
}
