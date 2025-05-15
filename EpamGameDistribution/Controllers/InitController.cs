using Microsoft.AspNetCore.Mvc;

namespace EpamGameDistribution.Controllers;

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