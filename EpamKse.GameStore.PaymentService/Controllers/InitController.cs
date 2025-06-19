using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.PaymentService.Controllers;

[ApiController]
[Route("/")]
public class InitController : ControllerBase
{
    public InitController() {}
    
    [HttpGet("/health")]
    public IActionResult Health()
    {
        return Ok("Healthy");
    }
    
    [HttpGet("/sayhi-from-api")]
    public async Task<IActionResult> SayHi()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("http://gamestore-api:5186/sayhi");
        if (response.IsSuccessStatusCode)
        {
            return Ok(response.Content.ReadAsStringAsync().Result);
        }

        return NotFound();
    }
}