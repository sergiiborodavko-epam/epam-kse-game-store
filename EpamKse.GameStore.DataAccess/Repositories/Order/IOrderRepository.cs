using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.DataAccess.Repositories.Order;
using Domain.Entities;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync(int limit, int offset);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, int limit, int offset);
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(Order order);
    Task<Order?> UpdateAsync(Order order);
    Task DeleteAsync(Order order);
}