using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Route("init")]
public class InitController:ControllerBase
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Access")]
    public IActionResult TestController()
    {
        return Ok("start");
    }
}