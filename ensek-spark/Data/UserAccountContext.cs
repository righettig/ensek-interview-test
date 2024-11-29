using ensek_spark.Models;
using Microsoft.EntityFrameworkCore;

namespace ensek_spark.Data;

public class UserAccountContext : DbContext
{
    public UserAccountContext(DbContextOptions<UserAccountContext> options) : base(options) { }

    public DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>()
            .HasNoDiscriminator()
            .ToContainer("user-accounts")
            .HasPartitionKey(e => e.AccountId)
            .HasKey(e => e.AccountId);

        base.OnModelCreating(modelBuilder);
    }
}
