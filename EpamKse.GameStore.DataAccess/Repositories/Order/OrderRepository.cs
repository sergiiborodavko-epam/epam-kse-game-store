using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.DataAccess.Repositories.Order;
using Domain.Entities;

public class OrderRepository : IOrderRepository
{
    private readonly GameStoreDbContext _context;

    public OrderRepository(GameStoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders.Include(o => o.Games).ToListAsync();
    }
    public async Task<IEnumerable<Order>> GetAllAsync(int limit, int offset)
    {
        return await _context.Orders.Include(o => o.Games).Skip(offset).Take(limit).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
    {
        return await _context.Orders.Include(o => o.Games).Where(o => o.Status == status).ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders.Include(o => o.Games).FirstOrDefaultAsync(o => id == o.Id);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}