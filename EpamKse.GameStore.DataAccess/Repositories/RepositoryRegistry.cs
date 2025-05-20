namespace EpamKse.GameStore.DataAccess.Repositories;

using Microsoft.Extensions.DependencyInjection;

using Game;

public static class RepositoryRegistry {
    public static void AddRepositories(this IServiceCollection services) {
        services.AddScoped<IGameRepository, GameRepository>();
    }
}
