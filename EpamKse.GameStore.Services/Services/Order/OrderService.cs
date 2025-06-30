using EpamKse.GameStore.DataAccess.Repositories.Game;
using EpamKse.GameStore.DataAccess.Repositories.Order;
using EpamKse.GameStore.Domain.DTO.Order;
using EpamKse.GameStore.Domain.DTO.License;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions.Game;
using EpamKse.GameStore.Domain.Exceptions.Order;
using EpamKse.GameStore.Services.Services.License;

namespace EpamKse.GameStore.Services.Services.Order;


using DataAccess.Repositories.GameBan;
using DataAccess.Repositories.User;
using Domain.Entities;
using Domain.Exceptions.User;

public class OrderService : IOrderService {
    private readonly IOrderRepository _orderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ILicenseService _licenseService;
    private readonly IUserRepository _userRepository;
    private readonly IGameBanRepository _banRepository;


    public OrderService(IOrderRepository orderRepository, IGameRepository gameRepository, 
        IUserRepository userRepository, IGameBanRepository banRepository, ILicenseService licenseService) {
        _orderRepository = orderRepository;
        _gameRepository = gameRepository;
        _licenseService = licenseService;
        _userRepository = userRepository;
        _banRepository = banRepository;
    }

    public async Task<IEnumerable<Order>> GetOrders(OrdersQueryDto ordersQueryDto)
    {
        if (ordersQueryDto.Status != null)
        {
            return await _orderRepository.GetByStatusAsync(ordersQueryDto.Status!.Value);
        }

        if (ordersQueryDto.Limit is > 0)
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
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {

            throw new UserNotFoundException(userId);
        }

        var games = await ValidateGames(dto.GameIds);
        await ValidateGamesBansForUser(games, user.Country);
        ValidateGamesStock(games);
        
        var order = new Order {
            UserId = userId, 
            CreatedAt = DateTime.Now, 
            Status = OrderStatus.Created
        };
        
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
            var user = await _userRepository.GetByIdAsync(order.UserId);
            var games = await ValidateGames(dto.GameIds);

            await ValidateGamesBansForUser(games, user!.Country);
            ValidateGamesStock(games);
            
            order.Games = new List<Game>();
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

        if (dto.OrderStatus == OrderStatus.Payed)
        {
            await _licenseService.CreateLicense(new CreateLicenseDto { OrderId = id });
        }

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

            if (game.Stock <= 0)
            {
                throw new NoGamesLeftException(game.Id);
            }


            games.Add(game);
        }

        return games;
    }


    private async Task ValidateGamesBansForUser(List<Game> games, Countries userCountry) {
        var countryBans = await _banRepository.GetByCountryAsync(userCountry);
        var bannedGameIds = countryBans.Select(b => b.GameId).ToHashSet();
        
        var bannedGamesInOrder = games.Where(g => bannedGameIds.Contains(g.Id)).ToList();
        
        if (bannedGamesInOrder.Count != 0) {
            var bannedGameNames = string.Join(", ", bannedGamesInOrder.Select(g => g.Title));
            throw new BannedGamesInOrderException(bannedGameNames);
        }
    }

    private static void ValidateGamesStock(List<Game> games)
    {
        foreach (var game in games.Where(game => game.Stock <= 0))
        {
            throw new NoGamesLeftException(game.Id);
        }
    }
    
    private static decimal CalcTotalSum(IEnumerable<Game> games)
    {
        return games.Sum(game => game.Price);
    }
}
