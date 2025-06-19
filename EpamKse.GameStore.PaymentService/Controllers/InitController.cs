using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.PaymentService.Controllers;

[ApiController]
[Route("/")]
public class InitController : ControllerBase
{
    [HttpGet("/payment-service-health")]
    public IActionResult Health()
    {
        return Ok("Reached payment service");
    }
    
    [HttpGet("/try-reach-api")]
    public async Task<IActionResult> SayHi()
    {
        var httpClient = new HttpClient();
        var apiKey = Environment.GetEnvironmentVariable("PAYMENT_SERVICE_API_KEY")
            ?? throw new InvalidOperationException("PAYMENT_SERVICE_API_KEY environment variable is not set");
        
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        var response = await httpClient.GetAsync("http://gamestore-api:5186/api-health");
        if (response.IsSuccessStatusCode)
        {
            return Ok(response.Content.ReadAsStringAsync().Result);
        }

        return NotFound();
    }
}