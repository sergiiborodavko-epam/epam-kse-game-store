using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

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