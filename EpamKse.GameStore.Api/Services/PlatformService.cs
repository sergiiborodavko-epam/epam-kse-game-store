using EpamKse.GameStore.Api.DTO.Platform;
using EpamKse.GameStore.Api.Exceptions;
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

    public async Task<Platform> GetPlatformById(int id)
    {
        var platform = await _context.Platforms.FirstOrDefaultAsync(platform => platform.Id == id);
        if (platform == null)
        {
            throw new NotFoundException($"Platform with id {id} was not found");
        }
        
        return platform;
    }

    public async Task<Platform> GetPlatformByName(string name)
    {
        var platform = await _context.Platforms.FirstOrDefaultAsync(platform => platform.Name == name);
        if (platform == null)
        {
            throw new NotFoundException($"Platform with name {name} was not found");
        }
        
        return platform;
    }

    public async Task<Platform> CreatePlatform(CreatePlatformDto dto)
    {
        if (await _context.Platforms.AnyAsync(platform => platform.Name.ToLower() == dto.Name.ToLower()))
        {
            throw new ConflictException($"Platform with name {dto.Name} already exists");
        }
        
        var platform = new Platform()
        {
            Name = dto.Name.ToLower()
        };
        
        _context.Platforms.Add(platform);
        await _context.SaveChangesAsync();
        
        return platform;
    }

    public async Task<Platform> UpdatePlatform(int id, UpdatePlatformDto dto)
    {
        var platform = await _context.Platforms.FirstOrDefaultAsync(platform => platform.Id == id);
        if (platform == null)
        {
            throw new NotFoundException($"Platform with id {id} was not found");
        }
        
        platform.Name = dto.Name?.ToLower() ?? platform.Name;
        await _context.SaveChangesAsync();
        
        return platform;
    }

    public async Task<Platform> DeletePlatform(int id)
    {
        var platform = await _context.Platforms.FirstOrDefaultAsync(platform => platform.Id == id);
        if (platform == null)
        {
            throw new NotFoundException($"Platform with id {id} was not found");
        }
        
        _context.Platforms.Remove(platform);
        await _context.SaveChangesAsync();
        return platform;
    }
}