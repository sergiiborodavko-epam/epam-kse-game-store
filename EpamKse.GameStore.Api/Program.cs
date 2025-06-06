using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DotNetEnv;

using EpamKse.GameStore.Api.Infrastructure;
using EpamKse.GameStore.DataAccess.Repositories;
using EpamKse.GameStore.Services.Services;
using EpamKse.GameStore.Api.Filters;
using EpamKse.GameStore.DataAccess.Context;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
    ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is not set");

builder.Services.AddDbContext<GameStoreDbContext>(options => 
    options.UseSqlServer(connectionString)
);

builder.Services.AddControllers(options => {
    options.Filters.Add<CustomHttpExceptionFilter>(); }).ConfigureApiBehaviorOptions(options => {
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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseWhen(context => context.Request.Path.StartsWithSegments("/auth/refresh"), 
    appBuilder => { appBuilder.UseMiddleware<RefreshTokenValidator>(); }
);

app.UseAuthentication();
app.UseCors("AllowCredentials");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
