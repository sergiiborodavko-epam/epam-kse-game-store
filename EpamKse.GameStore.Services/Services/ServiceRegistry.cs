namespace EpamKse.GameStore.Services.Services;

using Microsoft.Extensions.DependencyInjection;

using Game;

public static class ServiceRegistry {
    public static void AddServices(this IServiceCollection services) {
        services.AddScoped<IGameService, GameService>();
    }
}
