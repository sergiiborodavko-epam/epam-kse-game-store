using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Route("init")]
public class InitController : ControllerBase {
    
    [HttpGet("/try-reach-payment-service")]
    public async Task<IActionResult> SayHi() {
        var httpClient = new HttpClient();
        
        var apiKey = Environment.GetEnvironmentVariable("PAYMENT_SERVICE_API_KEY")
                     ?? throw new InvalidOperationException("PAYMENT_SERVICE_API_KEY environment variable is not set");
        
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        
        var response = await httpClient.GetAsync("http://gamestore-payment-service:5172/payment-service-health");
        return Ok(response.Content.ReadAsStringAsync().Result);
    }
    
    [HttpGet("/api-health")]
    public Task<IActionResult> TestController() {
        return Task.FromResult<IActionResult>(Ok("Reached api"));
    }
}
