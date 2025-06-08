using EpamKse.GameStore.Domain.DTO.Role;
using EpamKse.GameStore.Domain.Entities;

namespace EpamKse.GameStore.Services.Services.Role;

public interface IRoleService
{
    Task<User> UpdateRole(UpdateRoleDto dto);
}