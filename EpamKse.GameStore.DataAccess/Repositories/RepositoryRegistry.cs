using EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;
using EpamKse.GameStore.DataAccess.Repositories.Genre;
using EpamKse.GameStore.DataAccess.Repositories.License;
using EpamKse.GameStore.DataAccess.Repositories.Order;
using EpamKse.GameStore.DataAccess.Repositories.User;
using EpamKse.GameStore.DataAccess.Repositories.Platform;
using EpamKse.GameStore.DataAccess.Repositories.Publisher;

namespace EpamKse.GameStore.DataAccess.Repositories;

using Microsoft.Extensions.DependencyInjection;

using Game;
using Genre;
using GameFile;

public static class RepositoryRegistry {
    public static void AddRepositories(this IServiceCollection services) {
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IGameFileRepository, GameFileRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IHistoricalPriceRepository, HistoricalPriceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ILicenseRepository, LicenseRepository>();
    }
}
