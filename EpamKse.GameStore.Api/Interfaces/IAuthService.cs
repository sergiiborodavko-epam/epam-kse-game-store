using EpamKse.GameStore.Api.DTO.Auth;

namespace EpamKse.GameStore.Api.Interfaces;

public interface IAuthService
{
    public Task<string> Register(RegisterDTO registerDto);
    
    public Task<string> Login(LoginDTO loginDto);
}