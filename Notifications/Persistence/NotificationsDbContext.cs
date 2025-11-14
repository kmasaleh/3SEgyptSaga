using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient; // Optional, for SQL Server types
using Microsoft.EntityFrameworkCore.SqlServer;
using MassTransit.SqlTransport;
using Notifications.Domain;
using Notifications.Application.Sagas; // Required for UseSqlServer extension method

namespace Notifications.Persistence;

public class EvenetEntityConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(e => e.CreatedAt).IsRequired(true);
        builder.Property(e => e.Type).IsRequired(true);
        builder.Property(e => e.Data).IsRequired(false);
        builder.Property(e => e.UserEmail).IsRequired(true);
    }
}
public class NewUserOnboardSagaDataEntityConfiguration : IEntityTypeConfiguration<NewUserOnboardSagaData>
{
    public void Configure(EntityTypeBuilder<NewUserOnboardSagaData> builder)
    {
        builder.HasKey(e => e.CorrelationId);
    }
}

public class NotificationsDbContext : DbContext
{
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
    {
    }
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<NewUserOnboardSagaData> NewUserOnboardSagas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EvenetEntityConfiguration());
        modelBuilder.ApplyConfiguration(new NewUserOnboardSagaDataEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}


    

