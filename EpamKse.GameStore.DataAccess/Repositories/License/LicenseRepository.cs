using EpamKse.GameStore.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.DataAccess.Repositories.License;
using Domain.Entities;

public class LicenseRepository : ILicenseRepository
{
    private readonly GameStoreDbContext _context;
    
    public LicenseRepository(GameStoreDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<License>> GetAllAsync()
    {
        return await _context.Licenses
            .Include(l => l.Order)
            .ThenInclude(o => o.Games)
            .Include(l => l.Order)
            .ThenInclude(o => o.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<License>> GetByUserIdAsync(int userId)
    {
        return await _context.Licenses
            .Include(l => l.Order)
            .ThenInclude(o => o.Games)
            .Include(l => l.Order)
            .ThenInclude(o => o.User)
            .Where(l=> userId == l.Order.UserId)
            .ToListAsync();   
    }

    public async Task<License?> GetByIdAsync(int id)
    {
        return await _context.Licenses
            .Include(l => l.Order)
            .ThenInclude(o => o.Games)
            .Include(l => l.Order)
            .ThenInclude(o => o.User)
            .FirstOrDefaultAsync(l => id == l.Id);
    }

    public async Task<License?> GetByOrderIdAsync(int orderId)
    {
        return await _context.Licenses
            .Include(l => l.Order)
            .ThenInclude(o => o.Games)
            .Include(l => l.Order)
            .ThenInclude(o => o.User)
            .FirstOrDefaultAsync(l => orderId == l.OrderId);   
    }

    public async Task<License?> GetByKeyAsync(string key)
    {
        return await _context.Licenses
            .Include(l => l.Order)
            .ThenInclude(o => o.Games)
            .Include(l => l.Order)
            .ThenInclude(o => o.User)
            .FirstOrDefaultAsync(l => key == l.Key);
    }

    public async Task<License> CreateAsync(License license)
    {  
        _context.Licenses.Add(license);
        await _context.SaveChangesAsync();
        return license;
    }
    

    public async Task DeleteAsync(License license)
    {
        _context.Licenses.Remove(license);
        await _context.SaveChangesAsync();
    }
}