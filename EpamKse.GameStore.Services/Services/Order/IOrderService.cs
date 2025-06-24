using EpamKse.GameStore.Domain.DTO.Order;
using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.Services.Services.Order;
using Domain.Entities;
public interface IOrderService
{
    Task<IEnumerable<Order>> GetOrders(OrdersQueryDto dto);
    Task<Order> GetOrderById(int id);
    Task<Order> CreateOrder(int userId, CreateOrderDto dto);
    Task<Order> UpdateOrder(int id, UpdateOrderDto dto);
    Task<Order> DeleteOrder(int id);
    Task<OrderStatus> ProcessWebhook(int id, WebhookMessage dto);
}