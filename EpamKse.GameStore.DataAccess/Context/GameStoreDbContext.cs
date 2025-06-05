namespace EpamKse.GameStore.DataAccess.Context;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

public class GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<User> Users { get; set; }
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<HistoricalPrice> HistoricalPrices { get; set; }
    public DbSet<Publisher> Publishers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>().HasData(
            new Game { Id = 1, Title = "Game 1", Description = "Description for Game 1", Price = 49.99m },
            new Game { Id = 2, Title = "Game 2", Description = "Description for Game 2", Price = 59.99m }
        );

        modelBuilder.Entity<Platform>().HasData(
            new Platform { Id = 1, Name = "android" },
            new Platform { Id = 2, Name = "ios" },
            new Platform { Id = 3, Name = "windows" },
            new Platform { Id = 4, Name = "vr" }
        );
    }
}