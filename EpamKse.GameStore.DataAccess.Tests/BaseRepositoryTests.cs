namespace EpamKse.GameStore.DataAccess.Tests;

using Microsoft.EntityFrameworkCore;

using Context;
using Domain.Entities;

public abstract class BaseRepositoryTests : IDisposable {
    protected readonly GameStoreDbContext DbContext;

    protected BaseRepositoryTests() {
        var databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        DbContext = new GameStoreDbContext(options);
        SeedData();
    }

    private void SeedData() {
        DbContext.Genres.AddRange(
            new Genre { Id = 1, Name = "Strategy" },
            new Genre { Id = 2, Name = "Action" }
        );
        
        DbContext.Games.AddRange(
            new Game { Id = 1, Title = "Test Game 1", 
                Description = "Test Description 1", Price = 19.99m, 
                ReleaseDate = new DateTime(2023, 1, 1),
                GenreIds = [1]
            },
            new Game { Id = 2, Title = "Test Game 2", 
                Description = "Test Description 2", Price = 29.99m, 
                ReleaseDate = new DateTime(2023, 2, 1),
                GenreIds = [2]
            }
        );
        DbContext.SaveChanges();
    }

    public void Dispose() {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}
