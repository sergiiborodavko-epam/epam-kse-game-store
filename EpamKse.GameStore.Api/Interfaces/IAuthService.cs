using EpamKse.GameStore.Api.DTO.Auth;

namespace EpamKse.GameStore.Api.Interfaces;

public interface IAuthService
{
    public Task<(string, string)> Register(RegisterDTO registerDto);
    
    public Task<(string, string)> Login(LoginDTO loginDto);
    public Task<(string, string)> Refresh(string email);
}