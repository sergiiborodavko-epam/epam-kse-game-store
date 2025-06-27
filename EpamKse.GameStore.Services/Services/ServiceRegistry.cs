using EpamKse.GameStore.Services.Services.Encryption;
using EpamKse.GameStore.Services.Services.HistoricalPrice;
using EpamKse.GameStore.Services.Services.License;
using EpamKse.GameStore.Services.Services.Order;
using EpamKse.GameStore.Services.Services.Role;

namespace EpamKse.GameStore.Services.Services;

using Microsoft.Extensions.DependencyInjection;

using Game;
using Genre;
using Auth;
using Platform;
using Publisher;
using GameFile;
using GameBan;

public static class ServiceRegistry {
    public static void AddServices(this IServiceCollection services) {
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGameBanService, GameBanService>();
        services.AddScoped<IGameFileService, GameFileService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<IHistoricalPriceService, HistoricalPriceService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<ILicenseService, LicenseService>();
        services.AddSingleton<ILicenseBuilder, LicenseBuilder>();
    }
}
