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
using Domain.Exceptions.GameBan;
using Helpers.Auth;
using Helpers.GameBan;
using DataAccess.Context;

public class AuthService : IAuthService
{
    private readonly GameStoreDbContext _dbContext;
    private readonly string ACCESS_TOKEN_SECRET;
    private readonly string REFRESH_TOKEN_SECRET;
    private readonly int REFRESH_TOKEN_LIFETIME = 60;
    private readonly int ACCESS_TOKEN_LIFETIME = 30;

    public AuthService(GameStoreDbContext dbContext)
    {
        _dbContext = dbContext;
        REFRESH_TOKEN_SECRET = Environment.GetEnvironmentVariable("REFRESH_TOKEN_SECRET");
        ACCESS_TOKEN_SECRET = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET");
    }

    public async Task<(string, string)> Register(RegisterDTO registerDto)
    {
        if (!CountryHelper.IsValidCountry(registerDto.Country)) {
            throw new InvalidCountryException(registerDto.Country, CountryHelper.GetValidCountries());
        }
        
        if (await _dbContext.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            throw new UserAlreadyExistsException();
        }

        var hashedPassword = AuthHelper.HashPassword(registerDto.Password);
        var newUser = new User
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            PasswordHash = hashedPassword,
            FullName = registerDto.FullName,
            Role = Roles.Customer,
            Country = CountryHelper.ParseCountry(registerDto.Country),
        };
        
        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();
        var accessToken = GenerateAccessToken(newUser.Id, newUser.Email, newUser.Role.ToString());
        var refreshToken = GenerateRefreshToken(newUser.Id, newUser.Email, newUser.Role.ToString());

        return (accessToken, refreshToken);
    }

    public async Task<(string, string)> Login(LoginDTO loginDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user is not null && VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            var accessToken = GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
            var refreshToken = GenerateRefreshToken(user.Id, user.Email, user.Role.ToString());
            return (accessToken, refreshToken);
        }

        throw new InvalidCredentialsException();
    }

    public async Task<(string, string)> Refresh(ClaimsPrincipal principal)
    {
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var role = principal.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            throw new InvalidRefreshTokenException();

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is not null)
        {
            var accessToken = GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
            var refreshToken = GenerateRefreshToken(user.Id, user.Email, user.Role.ToString());
            return (accessToken, refreshToken);
        }

        throw new InvalidCredentialsException();
    }

    private string GenerateRefreshToken(int userId, string userEmail, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(REFRESH_TOKEN_SECRET));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("id", userId.ToString()),
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(REFRESH_TOKEN_LIFETIME),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateAccessToken(int userId, string userEmail, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ACCESS_TOKEN_SECRET));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("id", userId.ToString()),
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(ACCESS_TOKEN_LIFETIME),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return Argon2.Verify(hash, password);
    }
}
