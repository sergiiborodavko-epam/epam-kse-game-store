using EpamKse.GameStore.Domain.DTO.Role;
using EpamKse.GameStore.Services.Services.Role;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Route("roles")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("change")]
    public async Task<IActionResult> UpdateRole(UpdateRoleDto dto)
    {
        var user = await _roleService.UpdateRole(dto);
        return Ok(new { id = user.Id, role = user.Role });
    }
}