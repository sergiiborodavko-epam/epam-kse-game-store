using EpamGameDistribution.Services;
using EpamKse.GameStore.Api.DTO.Auth;
using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.DataAccess.Entities;
using EpamKse.GameStore.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
namespace EpamKse.GameStore.Tests;

public class AuthServiceTests
{
    private GameStoreDbContext _dbContext;
    private AuthService _authService;

    private const string TestSecret = "1JrdlK1ebzvcEr611SkJBBKgFtWvuEzqD-qC3DYssfj5NwZCqWEmn3fTOQphK-wWYghS5g3T-FQaHEbGzxIeTQ==";

    [SetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("AT_SECRET", TestSecret);

        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new GameStoreDbContext(options);
        _authService = new AuthService(_dbContext);
    }

    [Test]
    public async Task Register_CreatesUserAndReturnToken()
    {
        var dto = new RegisterDTO
        {
            Email = "test@example.com",
            Password = "Test123!",
            FullName = "FullName",
            UserName = "UserName"
        };

        var token = await _authService.Register(dto);

        Assert.IsFalse(string.IsNullOrEmpty(token));
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        Assert.IsNotNull(user);
        Assert.AreEqual("UserName", user.UserName);
    }

    [Test]
    public async Task Register_ExistingEmail_ReturnsEmptyToken()
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

        var token = await _authService.Register(dto);

        Assert.IsTrue(string.IsNullOrEmpty(token));
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var password = "Test123!";
        var hashed = typeof(AuthService)
            .GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { password }) as string;

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
        var token = await _authService.Login(dto);
        Assert.IsFalse(string.IsNullOrEmpty(token));
    }

    [Test]
    public async Task Login_WrongPassword_ReturnsEmptyToken()
    {
        var hashed = typeof(AuthService)
            .GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .Invoke(null, new object[] { "Correct123" }) as string;

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
        var token = await _authService.Login(dto);
        Assert.IsTrue(string.IsNullOrEmpty(token));
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }
}