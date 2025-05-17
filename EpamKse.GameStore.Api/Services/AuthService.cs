using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EpamKse.GameStore.Api.DTO.Auth;
using EpamKse.GameStore.Api.Interfaces;
using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.DataAccess.Entities;
using EpamKse.GameStore.DataAccess.Enums;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace EpamGameDistribution.Services;

public class AuthService:IAuthService
{
    private readonly GameStoreDbContext _dbContext;
    private readonly string AT_SECRET;
    public AuthService(GameStoreDbContext dbContext)
    {
        _dbContext = dbContext;
        AT_SECRET = Environment.GetEnvironmentVariable("AT_SECRET");
    }
    public async Task<string> Register(RegisterDTO registerDto)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            return "";
        }
        var hashedPassword = HashPassword(registerDto.Password);
        var at = GenerateAt(registerDto.Email, Roles.Customer.ToString());
        await _dbContext.Users.AddAsync(new User
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            PasswordHash = hashedPassword,
            FullName = registerDto.FullName,
            Role = Roles.Customer,
            
          
        });
        await _dbContext.SaveChangesAsync();
       
        return at;
    }

    public async Task<string> Login(LoginDTO loginDto)
    {
        var user =await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if ( user is not null && VerifyPassword(loginDto.Password,user.PasswordHash))
        { 
            return GenerateAt(loginDto.Email, user.Role.ToString());
        }
        return "";
    }
    private string GenerateAt(string userEmail,string role)
    {
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AT_SECRET));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role,role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
        
    }
    private static string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return Argon2.Verify(hash, password);
    }
}