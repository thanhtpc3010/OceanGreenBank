using Microsoft.EntityFrameworkCore;
using ProjectService.Domain.Entity;

namespace ProjectService.Infrastructure.Persistence.Contexts;

/// <summary>
/// Read DbContext — dùng cho Query (GET), cấu hình NoTracking để tối ưu đọc.
/// </summary>
public class ApplicationReadDbContext(DbContextOptions<ApplicationReadDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<SavingsPlan> SavingsPlans { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<AutoEarnSetting> AutoEarnSettings { get; set; }
    public DbSet<AutoEarnLog> AutoEarnLogs { get; set; }
    public DbSet<KnowledgeEntry> KnowledgeEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationReadDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
