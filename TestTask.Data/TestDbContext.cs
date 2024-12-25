using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTask.Data.Entities;

namespace TestTask.Data;
public class DbConfig
{
    public required string ConnectionString { get; set; }
    public bool Reset { get; set; }
}
public static class DbContextExtensions
{
    public static async Task MigrateDatabaseAsync<TContext>(this IHost webApp) where TContext : DbContext
    {
        await using var scope = webApp.Services.CreateAsyncScope();
        await using var appContext = scope.ServiceProvider.GetRequiredService<TContext>();
        var config = scope.ServiceProvider.GetRequiredService<DbConfig>();
        if (config.Reset)
            await appContext.Database.EnsureDeletedAsync();

        // var pendingMigrations = (await appContext.Database.GetPendingMigrationsAsync()).ToList();
        // logger.LogInformation("Pending Migrations ({Count}):\n{Data}", pendingMigrations.Count,
        //     string.Join("\n", pendingMigrations));
        //
        await appContext.Database.MigrateAsync();
        // var appliedMigrations = (await appContext.Database.GetAppliedMigrationsAsync()).ToList();
        // logger.LogInformation("Applied Migrations ({Count}):\n{Data}", appliedMigrations.Count,
        //     string.Join("\n", appliedMigrations));
    }
}
public class TestDbContext : DbContext
{
    private readonly bool _initialized;

    public TestDbContext()
    {
    }
    
    
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
        _initialized = true;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<UserItem> UserItems { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!_initialized)
            optionsBuilder.UseNpgsql();
    }
}