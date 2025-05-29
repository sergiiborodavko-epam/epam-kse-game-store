using EpamKse.GameStore.DataAccess.Repositories.User;
using EpamKse.GameStore.Domain.DTO.Role;
using EpamKse.GameStore.Domain.Entities;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions;
using EpamKse.GameStore.Services.Services.Role;
using Moq;
using Xunit;

namespace EpamKse.GameStore.Services.Tests.Services;

public class RoleServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly RoleService _roleService;
    
    public RoleServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleService = new RoleService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task UpdateRole_ShouldChangeRole_WhenUserExists()
    {
        var mockUser = new User { Id = 1, Email = "test@test.com", Role = Roles.Customer };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(mockUser);

        var user = await _roleService.UpdateRole(new UpdateRoleDto { UserId = 1, Role = Roles.Admin });
        
        Assert.Equal(1, user.Id);
        Assert.Equal(Roles.Admin, user.Role);
    }

    [Fact]
    public async Task UpdateRole_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var mockUser = new User { Id = 1, Email = "test@test.com", Role = Roles.Customer };
        _userRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(mockUser);
        
        await Assert.ThrowsAsync<NotFoundException>(() => _roleService.UpdateRole(new UpdateRoleDto { UserId = 9999, Role = Roles.Admin })) ;
    }
}