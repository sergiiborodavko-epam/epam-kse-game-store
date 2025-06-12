using EpamKse.GameStore.DataAccess.Repositories.Game;
using EpamKse.GameStore.DataAccess.Repositories.Order;
using EpamKse.GameStore.Domain.DTO.Order;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions.Game;
using EpamKse.GameStore.Domain.Exceptions.Order;

namespace EpamKse.GameStore.Services.Services.Order;
using Domain.Entities;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGameRepository _gameRepository;

    public OrderService(IOrderRepository orderRepository, IGameRepository gameRepository)
    {
        _orderRepository = orderRepository;
        _gameRepository = gameRepository;
    }

    public async Task<IEnumerable<Order>> GetOrders(OrdersQueryDto ordersQueryDto)
    {
        if (ordersQueryDto.Status != null)
        {
            return await _orderRepository.GetByStatusAsync(ordersQueryDto.Status!.Value);
        }

        if (ordersQueryDto.Limit != null && ordersQueryDto.Limit > 0)
        {
            return await _orderRepository.GetAllAsync(ordersQueryDto.Limit!.Value, ordersQueryDto.Offset);
        }
        
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order> GetOrderById(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            throw new OrderNotFoundException(id);
        }
        return order;
    }

    public async Task<Order> CreateOrder(int userId, CreateOrderDto dto)
    {
        var order = new Order()
        {
            UserId = userId, 
            CreatedAt = DateTime.Now, 
            Status = OrderStatus.Created
        };

        await AddGamesToOrder(order, dto.GameIds);
        
        await _orderRepository.CreateAsync(order);
        return order;
    }

    public async Task<Order> UpdateOrder(int id, UpdateOrderDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            throw new OrderNotFoundException(id);
        }

        order.Status = dto.Status ?? order.Status;
        if (dto.GameIds != null)
        {
            order.TotalSum = 0;
            order.Games = new List<Game>();
            await AddGamesToOrder(order, dto.GameIds);
        }

        await _orderRepository.UpdateAsync(order);
        return order;
    }

    public async Task<Order> DeleteOrder(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            throw new OrderNotFoundException(id);
        }
        await _orderRepository.DeleteAsync(order);
        return order;
    }

    private async Task AddGamesToOrder(Order order, List<int> gameIds)
    {
        foreach (var gameId in gameIds)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new GameNotFoundException(gameId);
            }
            order.TotalSum += game.Price;
            order.Games.Add(game);
        }
    }
}