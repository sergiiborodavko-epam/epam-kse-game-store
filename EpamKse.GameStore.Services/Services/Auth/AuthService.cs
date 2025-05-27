namespace EpamKse.GameStore.Services.Services.Auth;

using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Domain.DTO.Auth;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions.Auth;
using Helpers.Auth;
using DataAccess.Context;

public class AuthService : IAuthService
{
    private readonly GameStoreDbContext _dbContext;
    private readonly string ACCESS_TOKEN_SECRET;

    public AuthService(GameStoreDbContext dbContext)
    {
        _dbContext = dbContext;
        ACCESS_TOKEN_SECRET = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET");
    }

    public async Task<string> Register(RegisterDTO registerDto)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            throw new UserAlreadyExistsException();
        }

        var hashedPassword = AuthHelper.HashPassword(registerDto.Password);
        var accessToken = GenerateAccessToken(registerDto.Email, Roles.Customer.ToString());
        await _dbContext.Users.AddAsync(new User
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            PasswordHash = hashedPassword,
            FullName = registerDto.FullName,
            Role = Roles.Customer,
        });
        await _dbContext.SaveChangesAsync();

        return accessToken;
    }

    public async Task<string> Login(LoginDTO loginDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user is not null && VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return GenerateAccessToken(loginDto.Email, user.Role.ToString());
        }

        throw new InvalidCredentialsException();
    }

    private string GenerateAccessToken(string userEmail, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ACCESS_TOKEN_SECRET));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return Argon2.Verify(hash, password);
    }
}