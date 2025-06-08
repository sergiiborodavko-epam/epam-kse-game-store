using EpamKse.GameStore.DataAccess.Repositories.User;
using EpamKse.GameStore.Domain.DTO.Role;
using EpamKse.GameStore.Domain.Entities;
using EpamKse.GameStore.Domain.Exceptions;
using EpamKse.GameStore.Domain.Exceptions.User;

namespace EpamKse.GameStore.Services.Services.Role;

public class RoleService : IRoleService
{
    private readonly IUserRepository _userRepository;

    public RoleService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public async Task<User> UpdateRole(UpdateRoleDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId!.Value);
        if (user == null) throw new UserNotFoundException(dto.UserId!.Value);
        user.Role = dto.Role!.Value;
        await _userRepository.UpdateAsync(user);
        return user;
    }
}