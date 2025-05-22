using DotNetEnv;
using Microsoft.EntityFrameworkCore;

using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.DataAccess.Repositories;
using EpamKse.GameStore.Services.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
    ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is not set");

builder.Services.AddDbContext<GameStoreDbContext>(options => 
    options.UseSqlServer(connectionString)
);

builder.Services.AddControllers().
    ConfigureApiBehaviorOptions(options => {
    options.SuppressModelStateInvalidFilter = false;
});

builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
