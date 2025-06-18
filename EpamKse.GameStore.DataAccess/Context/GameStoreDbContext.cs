using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.DataAccess.Context;

using System.Reflection;
using Microsoft.EntityFrameworkCore;

using Domain.Entities;

public class GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : DbContext(options) {
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<User> Users { get; set; }
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<HistoricalPrice> HistoricalPrices { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<GameFile> GameFiles { get; set; }

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
            new Genre { Id = 1, Name = "Strategy", ParentGenreId = null },
            new Genre { Id = 2, Name = "RTS", ParentGenreId = 1 },
            new Genre { Id = 3, Name = "TBS", ParentGenreId = 1 },
            new Genre { Id = 4, Name = "RPG", ParentGenreId = null },
            new Genre { Id = 5, Name = "Sports", ParentGenreId = null },
            new Genre { Id = 6, Name = "Races", ParentGenreId = 5 },
            new Genre { Id = 7, Name = "Rally", ParentGenreId = 5 },
            new Genre { Id = 8, Name = "Arcade", ParentGenreId = 5 },
            new Genre { Id = 9, Name = "Formula", ParentGenreId = 5 },
            new Genre { Id = 10, Name = "Off-road", ParentGenreId = 5 },
            new Genre { Id = 11, Name = "Action", ParentGenreId = null },
            new Genre { Id = 12, Name = "FPS", ParentGenreId = 11 },
            new Genre { Id = 13, Name = "TPS", ParentGenreId = 11 },
            new Genre { Id = 14, Name = "Adventure", ParentGenreId = 11 },
            new Genre { Id = 15, Name = "Puzzle & Skill", ParentGenreId = null }
        );

        // the safest way of doing this with migration is already hashed password
        var adminPass =
            "$argon2id$v=19$m=65536,t=3,p=1$hIWcROP/j0uU/PceT+/jHw$Kn1RHnAoDdMitEPzaT43//MwsEDJMwAjEPr8liXCHrM";
        
        modelBuilder.Entity<User>().HasData(
            new User { 
                Id = 1, 
                Role = Roles.Admin, 
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), 
                Email = "admin@example.com", 
                UserName = "admin", 
                FullName = "admin", 
                PasswordHash = adminPass 
            }
        );
    }
}
