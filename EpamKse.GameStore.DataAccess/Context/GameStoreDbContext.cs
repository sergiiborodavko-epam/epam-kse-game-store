namespace EpamKse.GameStore.DataAccess.Context;

using System.Reflection;
using Microsoft.EntityFrameworkCore;

using Domain.Entities;

public class GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : DbContext(options) {
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<User> Users { get; set; }
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Game>().HasData(
            new Game { Id = 1, Title = "Game 1", Description = "Description for Game 1", Price = 49.99m, GenreIds = [1, 2, 4] },
            new Game { Id = 2, Title = "Game 2", Description = "Description for Game 2", Price = 59.99m, GenreIds = [11, 13] }
        );

        modelBuilder.Entity<Platform>().HasData(
            new Platform { Id = 1, Name = "android" },
            new Platform { Id = 2, Name = "ios" },
            new Platform { Id = 3, Name = "windows" },
            new Platform { Id = 4, Name = "vr" }
        );

        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Strategy" },
            new Genre { Id = 2, Name = "RTS" },
            new Genre { Id = 3, Name = "TBS" },
            new Genre { Id = 4, Name = "RPG" },
            new Genre { Id = 5, Name = "Sports" },
            new Genre { Id = 6, Name = "Races" },
            new Genre { Id = 7, Name = "Rally" },
            new Genre { Id = 8, Name = "Arcade" },
            new Genre { Id = 9, Name = "Formula" },
            new Genre { Id = 10, Name = "Off-road" },
            new Genre { Id = 11, Name = "Action" },
            new Genre { Id = 12, Name = "FPS" },
            new Genre { Id = 13, Name = "TPS" },
            new Genre { Id = 14, Name = "Adventure" },
            new Genre { Id = 15, Name = "Puzzle & Skill" }
        );
    }
}
