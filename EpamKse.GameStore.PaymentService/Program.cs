using DotNetEnv;
using EpamKse.GameStore.Domain.Policies;
using EpamKse.GameStore.PaymentService.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Apikey", new OpenApiSecurityScheme
    {
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Apikey"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers(options =>
{
    var apiKey = Environment.GetEnvironmentVariable("PAYMENT_SERVICE_API_KEY") 
        ?? throw new InvalidOperationException("PAYMENT_SERVICE_API_KEY environment variable is not set");
    
    options.Filters.Add(new ApikeyFilter(apiKey));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
