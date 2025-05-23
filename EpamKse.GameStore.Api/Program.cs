using System.Text;
using DotNetEnv;
using EpamGameDistribution.Services;
using EpamKse.GameStore.Api.Infrastructure;
using EpamKse.GameStore.Api.Interfaces;
using EpamKse.GameStore.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAuthentication()
    .AddJwtBearer("Access", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")))
        };
    });
builder.Services.AddControllers();
builder.Services.AddDbContext<GameStoreDbContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING")));
builder.Services.AddScoped<IAuthService, AuthService>();
var app = builder.Build();
app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/auth/refresh"),
    appBuilder => { appBuilder.UseMiddleware<RefreshTokenValidator>(); });

app.UseAuthentication();
app.UseCors("AllowCredentials");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();