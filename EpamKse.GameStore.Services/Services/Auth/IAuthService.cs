using System.Security.Claims;
using EpamKse.GameStore.Domain.DTO.Auth;

namespace EpamKse.GameStore.Services.Services.Auth;

public interface IAuthService
{
    public Task<(string, string)> Register(RegisterDTO registerDto);

    public Task<(string, string)> Login(LoginDTO loginDto);
    public Task<(string, string)> Refresh(ClaimsPrincipal principal);
}