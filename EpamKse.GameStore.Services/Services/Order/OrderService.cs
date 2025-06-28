using EpamKse.GameStore.DataAccess.Repositories.Game;
using EpamKse.GameStore.DataAccess.Repositories.Order;
using EpamKse.GameStore.Domain.DTO.License;
using EpamKse.GameStore.Domain.DTO.Order;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions.Game;
using EpamKse.GameStore.Domain.Exceptions.Order;
using EpamKse.GameStore.Services.Services.License;

namespace EpamKse.GameStore.Services.Services.Order;
using Domain.Entities;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ILicenseService _licenseService;

    public OrderService(IOrderRepository orderRepository, IGameRepository gameRepository, ILicenseService licenseService)
    {
        _orderRepository = orderRepository;
        _gameRepository = gameRepository;
        _licenseService = licenseService;
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

        var games = await ValidateGames(dto.GameIds);
        
        foreach (var game in games)
        {
            order.Games.Add(game);
            game.Stock--;
        }
        
        order.TotalSum = CalcTotalSum(games);
        
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
        foreach (var oldGame in order.Games)
        {
            oldGame.Stock++;
        }
        order.Status = dto.Status ?? order.Status;
        if (dto.GameIds != null)
        {
            order.Games = new List<Game>();
            var games = await ValidateGames(dto.GameIds);
            
            foreach (var game in games)
            {
                order.Games.Add(game);
                game.Stock--;
            }
        
            order.TotalSum = CalcTotalSum(games);
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
        foreach (var game in order.Games)
        {
            game.Stock++;
        }

        await _orderRepository.DeleteAsync(order);
        return order;
    }

    public async Task<OrderStatus> ProcessWebhook(int id, WebhookMessage dto)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
        {
            throw new OrderNotFoundException(id);
        }

        order.Status = dto.OrderStatus;

       // if (dto.OrderStatus == OrderStatus.Payed)
        //{
       //     await _licenseService.CreateLicense(new CreateLicenseDto { OrderId = id });
       // }
        
        await _orderRepository.UpdateAsync(order);
        return order.Status;
    }

    private async Task<List<Game>> ValidateGames(List<int> gameIds)
    {
        var games = new List<Game>();
        foreach (var gameId in gameIds)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new GameNotFoundException(gameId);
            }
            if (game.Stock<=0)
            {
                throw new NoGamesLeftException(game.Id);
            }
            games.Add(game);
        }
        return games;
    } 
    private decimal CalcTotalSum(IEnumerable<Game> games)
    {
        return games.Sum(game => game.Price);
    }
}