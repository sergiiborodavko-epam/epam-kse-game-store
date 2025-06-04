using EpamKse.GameStore.DataAccess.Repositories.Platform;
using EpamKse.GameStore.DataAccess.Repositories.Publisher;

namespace EpamKse.GameStore.DataAccess.Repositories;

using Microsoft.Extensions.DependencyInjection;
using Game;

public static class RepositoryRegistry
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
    }
}