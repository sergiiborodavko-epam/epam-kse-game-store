namespace EpamKse.GameStore.Tests;

using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

using EpamKse.GameStore.Services.Services.Auth;
using Domain.DTO.Auth;
using Domain.Exceptions.Auth;
using Domain.Enums;
using Domain.Entities;
using Services.Helpers.Auth;
using DataAccess.Context;



public class AuthServiceTests
{
    private GameStoreDbContext _dbContext;
    private AuthService _authService;

    private const string AccessSecret =
        "1JrdlK1ebzvcEr611SkJBBKgFtWvuEzqD-qC3DYssfj5NwZCqWEmn3fTOQphK-wWYghS5g3T-FQaHEbGzxIeTQ==";

    private const string RefreshSecret =
        "XWt973tFGfic1gI-MjiSclxa2YyPoAsLIe7E1wWw0BlN-kB5Yj98FwMIufzihrHxT89GY__RnR2PnIzdZKkq-Q==";

    [SetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_SECRET", AccessSecret);
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_SECRET", RefreshSecret);

        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new GameStoreDbContext(options);
        _authService = new AuthService(_dbContext);
    }

    [Test]
    public async Task Register_CreatesUserAndReturnsTokens()
    {
        var dto = new RegisterDTO
        {
            Email = "test@example.com",
            Password = "Test123!",
            FullName = "FullName",
            UserName = "UserName"
        };

        var (accessToken, refreshToken) = await _authService.Register(dto);

        Assert.IsFalse(string.IsNullOrEmpty(accessToken));
        Assert.IsFalse(string.IsNullOrEmpty(refreshToken));

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        Assert.IsNotNull(user);
        Assert.AreEqual("UserName", user.UserName);
    }

    [Test]
    public async Task Register_ExistingEmail_ThrowsUserAlreadyExistsException()
    {
        var existingUser = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashed",
            FullName = "FullName",
            UserName = "UserName",
            Role = Roles.Customer
        };
        await _dbContext.Users.AddAsync(existingUser);
        await _dbContext.SaveChangesAsync();

        var dto = new RegisterDTO
        {
            Email = "test@example.com",
            Password = "Test123!",
            FullName = "FullName",
            UserName = "UserName"
        };

        var ex = Assert.ThrowsAsync<UserAlreadyExistsException>(() => _authService.Register(dto));
        Assert.That(ex.Message, Is.EqualTo("User already exists."));
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        var hashed = AuthHelper.HashPassword("Test123!");

        var user = new User
        {
            Email = "login@example.com",
            PasswordHash = hashed,
            FullName = "FullName",
            UserName = "UserName",
            Role = Roles.Customer
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var dto = new LoginDTO
        {
            Email = "login@example.com",
            Password = "Test123!"
        };

        var (accessToken, refreshToken) = await _authService.Login(dto);

        Assert.IsFalse(string.IsNullOrEmpty(accessToken));
        Assert.IsFalse(string.IsNullOrEmpty(refreshToken));
    }

    [Test]
    public async Task Login_WrongPassword_ThrowsInvalidCredentialsException()
    {
        var hashed = AuthHelper.HashPassword("Correct123");

        var user = new User
        {
            Email = "Email@example.com",
            PasswordHash = hashed,
            FullName = "FullName",
            UserName = "UserName",
            Role = Roles.Customer
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var dto = new LoginDTO
        {
            Email = "Email@example.com",
            Password = "wergthntgfd"
        };
        var ex = Assert.ThrowsAsync<InvalidCredentialsException>(() => _authService.Login(dto));
        Assert.That(ex.Message, Is.EqualTo("Invalid email or password."));
    }

    [Test]
    public async Task Refresh_ValidUser_ReturnsNewTokens()
    {
        var user = new User
        {
            Email = "refresh@example.com",
            PasswordHash = "331rf",
            FullName = "FullName",
            UserName = "UserName",
            Role = Roles.Customer
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, "RefreshToken");
        var principal = new ClaimsPrincipal(identity);

        var (accessToken, refreshToken) = await _authService.Refresh(principal);
        Assert.IsFalse(string.IsNullOrEmpty(accessToken));
        Assert.IsFalse(string.IsNullOrEmpty(refreshToken));
    }

    [Test]
    public void Refresh_InvalidEmail_ThrowsInvalidCredentialsException()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, "nonexistent@example.com"),
            new Claim(ClaimTypes.Role, "Customer")
        };
        var identity = new ClaimsIdentity(claims, "RefreshToken");
        var principal = new ClaimsPrincipal(identity);
        var ex = Assert.ThrowsAsync<InvalidCredentialsException>(() => _authService.Refresh(principal));
        Assert.That(ex.Message, Is.EqualTo("Invalid email or password."));
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }
}