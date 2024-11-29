using ensek_spark.Models;
using Microsoft.EntityFrameworkCore;

namespace ensek_spark.Data;

public class MeterReadingContext : DbContext
{
    public MeterReadingContext(DbContextOptions<MeterReadingContext> options) : base(options) { }

    public DbSet<MeterReading> MeterReadings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeterReading>()
            .HasNoDiscriminator()
            .ToContainer("meter-readings")
            .HasPartitionKey(e => e.AccountId)
            .HasKey(e => e.AccountId);

        base.OnModelCreating(modelBuilder);
    }
}
