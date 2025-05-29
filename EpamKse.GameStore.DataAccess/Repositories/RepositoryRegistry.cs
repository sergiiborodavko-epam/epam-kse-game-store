namespace EpamKse.GameStore.DataAccess.Repositories;

using Microsoft.Extensions.DependencyInjection;

using Game;
using Genre;

public static class RepositoryRegistry {
    public static void AddRepositories(this IServiceCollection services) {
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
    }
}
