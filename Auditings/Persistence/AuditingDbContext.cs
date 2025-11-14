using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Auditings.Domain; // Required for UseSqlServer extension method

namespace Auditings.Persistence;

public class LogEntityConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.HasKey(e => e.Id);
    }
}


public class AuditingDbContext : DbContext
{
    public AuditingDbContext(DbContextOptions<AuditingDbContext> options) : base(options)
    {
    }
    public DbSet<Log> Logs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new LogEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}


    

