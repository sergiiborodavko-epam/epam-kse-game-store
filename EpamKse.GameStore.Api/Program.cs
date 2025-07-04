using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.AspNetCore.Server.Kestrel.Core;

using EpamKse.GameStore.Api.Infrastructure;
using EpamKse.GameStore.DataAccess.Repositories;
using EpamKse.GameStore.Services.Services;
using EpamKse.GameStore.Api.Filters;
using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.Domain.Policies;
using EpamKse.GameStore.Domain.Profiles;
using Microsoft.AspNetCore.Authorization;

Env.Load();

var apiKey = Environment.GetEnvironmentVariable("PAYMENT_SERVICE_API_KEY") ??
             throw new InvalidOperationException("PAYMENT_SERVICE_API_KEY environment variable is not set");

const int maxRequestBodySizeBytes = 100 * 1024 * 1024;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
    ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is not set");

builder.Services.AddDbContext<GameStoreDbContext>(options => 
    options.UseSqlServer(connectionString)
);

builder.Services.Configure<IISServerOptions>(options => {
    options.MaxRequestBodySize = maxRequestBodySizeBytes;
});

builder.Services.Configure<KestrelServerOptions>(options => {
    options.Limits.MaxRequestBodySize = maxRequestBodySizeBytes;
});

builder.Services.AddHttpClient("PaymentServiceClient", client => {
    client.BaseAddress = new Uri(builder.Configuration["ServicesUrls:PaymentService"]!);
    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
});

builder.Services.AddAutoMapper(typeof(OrderProfile).Assembly);
builder.Services.AddControllers(options => {
    options.Filters.Add<CustomHttpExceptionFilter>();
}).AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}).ConfigureApiBehaviorOptions(options => {
    options.SuppressModelStateInvalidFilter = false;
});

builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication()
    .AddJwtBearer("Access", options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!))
        };
    });

builder.Services.AddSingleton<IAuthorizationHandler, ApikeyHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserPolicy", policy => policy.RequireAuthenticatedUser())
    .AddPolicy("ApikeyPolicy", policy => policy.Requirements.Add(new ApikeyRequirement(apiKey)));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "GameStore");
    options.ConfigObject.AdditionalItems["withCredentials"] = true;
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/auth/refresh"), 
    appBuilder => { appBuilder.UseMiddleware<RefreshTokenValidator>(); }
);

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowCredentials");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
