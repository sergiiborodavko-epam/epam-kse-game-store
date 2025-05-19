using DotNetEnv;
using EpamKse.GameStore.Api.Filters;
using EpamKse.GameStore.Api.Interfaces;
using EpamKse.GameStore.Api.Services;
using EpamKse.GameStore.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomHttpExceptionFilter>();
});
builder.Services.AddDbContext<GameStoreDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING")));
builder.Services.AddScoped<IPlatformService, PlatformService>();
var app = builder.Build();
app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.Run();