using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EMailService.Domain; // Required for UseSqlServer extension method

namespace EMailService.Persistence;

public class EMailEntityConfiguration : IEntityTypeConfiguration<EMail>
{
    public void Configure(EntityTypeBuilder<EMail> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(e => e.Subject).IsRequired();
        builder.Property(e => e.Body).IsRequired();
        builder.Property(e => e.To).IsRequired();
    }
}


public class EMailDbContext : DbContext
{
    public EMailDbContext(DbContextOptions<EMailDbContext> options) : base(options)
    {
    }
    public DbSet<EMail> EMails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EMailEntityConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}


    

