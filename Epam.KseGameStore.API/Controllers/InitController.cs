using Microsoft.AspNetCore.Mvc;

namespace Epam.KseGameStore.API.Controllers;

[ApiController]
[Route("init")]
public class InitController:ControllerBase
{
    [HttpGet]
    public IActionResult TestController()
    {
        return Ok("start");
    }
}