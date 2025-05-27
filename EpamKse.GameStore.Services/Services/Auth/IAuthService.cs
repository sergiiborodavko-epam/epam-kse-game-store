using EpamKse.GameStore.Domain.DTO.Auth;

namespace EpamKse.GameStore.Services.Services.Auth;

public interface IAuthService
{
    public Task<string> Register(RegisterDTO registerDto);
    
    public Task<string> Login(LoginDTO loginDto);
}