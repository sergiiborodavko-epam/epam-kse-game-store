using EpamKse.GameStore.Api.DTO.Platform;
using EpamKse.GameStore.Api.Interfaces;
using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.Api.Services;

public class PlatformService : IPlatformService
{
    private readonly GameStoreDbContext _context;

    public PlatformService(GameStoreDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Platform>> GetPlatforms()
    {
        return await _context.Platforms.ToListAsync();
    }

    public async Task<Platform?> GetPlatformById(int id)
    {
        return await _context.Platforms.FirstOrDefaultAsync(platform => platform.Id == id);
    }

    public async Task<Platform?> GetPlatformByName(string name)
    {
        return await _context.Platforms.FirstOrDefaultAsync(platform => platform.Name == name);
    }

    public async Task<Platform> CreatePlatform(CreatePlatformDto dto)
    {
        var platform = new Platform()
        {
            Name = dto.Name,
        };
        
        _context.Platforms.Add(platform);
        await _context.SaveChangesAsync();
        
        return platform;
    }

    public async Task<Platform?> UpdatePlatform(int id, UpdatePlatformDto dto)
    {
        var platform = await _context.Platforms.FirstOrDefaultAsync(platform => platform.Id == id);
        if (platform == null)
        {
            return null;
        }
        
        platform.Name = dto.Name ?? platform.Name;
        await _context.SaveChangesAsync();
        
        return platform;
    }

    public async Task<Platform?> DeletePlatform(int id)
    {
        var platform = await _context.Platforms.FirstOrDefaultAsync(platform => platform.Id == id);
        if (platform == null)
        {
            return null;
        }
        
        _context.Platforms.Remove(platform);
        await _context.SaveChangesAsync();
        return platform;
    }
}