using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("init")]
public class InitController:ControllerBase
{
    [HttpGet("/healthcheck")]
    public async Task<IActionResult> TestController()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("http://gamestore-payment-service:5172/health");
        return Ok(response.Content.ReadAsStringAsync().Result);
    }
    
    [HttpGet("/sayhi")]
    public IActionResult SayHi()
    {
        return Ok("Hi!!!! I am up and running");
    }
}