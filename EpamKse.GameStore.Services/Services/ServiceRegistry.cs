﻿namespace EpamKse.GameStore.Services.Services;

using Microsoft.Extensions.DependencyInjection;

using Game;
using Genre;
using Auth;
using Platform;
using Publisher;

public static class ServiceRegistry {
    public static void AddServices(this IServiceCollection services) {
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IPublisherService, PublisherService>();
    }
}