using DotNetEnv;
using EpamKse.GameStore.PaymentService.Filters;
using EpamKse.GameStore.PaymentService.Services.Payments;

Env.Load();

var apiKey = Environment.GetEnvironmentVariable("PAYMENT_SERVICE_API_KEY") 
             ?? throw new InvalidOperationException("PAYMENT_SERVICE_API_KEY environment variable is not set");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(options => {
    options.Filters.Add(new CustomHttpExceptionFilter());
    options.Filters.Add(new ApikeyFilter(apiKey));
});

builder.Services.AddHttpClient("ApiClient", client => {
    client.BaseAddress = new Uri(builder.Configuration["ServicesUrls:Api"]!);
    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
