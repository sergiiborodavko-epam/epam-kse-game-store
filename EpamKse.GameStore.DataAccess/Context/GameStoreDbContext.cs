using System.Reflection;
using EpamKse.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.DataAccess.Context;

public class GameStoreDbContext:DbContext
{
    public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Platform> Platforms { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        modelBuilder.Entity<Platform>().HasData(
            new Platform { Id = 1, Name = "android" },
            new Platform { Id = 2, Name = "ios" },
            new Platform { Id = 3, Name = "windows" },
            new Platform { Id = 4, Name = "vr" }
        );

        base.OnModelCreating(modelBuilder);
    } 
}