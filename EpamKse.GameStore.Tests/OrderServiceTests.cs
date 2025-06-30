namespace EpamKse.GameStore.Tests;

using Xunit;
using Moq;

using DataAccess.Repositories.Game;
using DataAccess.Repositories.GameBan;
using DataAccess.Repositories.Order;
using DataAccess.Repositories.User;
using Domain.DTO.Order;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions.Game;
using Domain.Exceptions.Order;
using Domain.Exceptions.User;
using Domain.DTO.License;
using EpamKse.GameStore.Services.Services.License;
using EpamKse.GameStore.Services.Services.Order;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IGameBanRepository> _gameBanRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILicenseService> _licenseServiceMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _gameRepositoryMock = new Mock<IGameRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _gameBanRepositoryMock = new Mock<IGameBanRepository>();
        _licenseServiceMock = new Mock<ILicenseService>();
        _orderService = new OrderService(_orderRepositoryMock.Object, _gameRepositoryMock.Object,
            _userRepositoryMock.Object, _gameBanRepositoryMock.Object, _licenseServiceMock.Object);
    }

    [Fact]
    public async Task GetOrders_ReturnsAllOrders_WhenLimitIsInvalid()
    {
        var queryDto = new OrdersQueryDto { Limit = 0, Offset = 0 };
        var expectedOrders = new List<Order> { new Order() };
        _orderRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedOrders);

        var result = await _orderService.GetOrders(queryDto);

        Assert.Equal(expectedOrders, result);
        _orderRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetOrders_ReturnsOrders_ByStatusOrAll()
    {
        var status = OrderStatus.Created;
        var queryDto = new OrdersQueryDto { Status = status, Limit = 10, Offset = 0 };
        var expectedOrders = new List<Order> { new Order() };
        _orderRepositoryMock.Setup(r => r.GetByStatusAsync(status)).ReturnsAsync(expectedOrders);

        var result = await _orderService.GetOrders(queryDto);

        Assert.Equal(expectedOrders, result);
        _orderRepositoryMock.Verify(r => r.GetByStatusAsync(status), Times.Once);
    }

    [Fact]
    public async Task GetOrderById_ReturnsOrder_WhenOrderExists()
    {
        var orderId = 1;
        var expectedOrder = new Order { Id = orderId };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(expectedOrder);

        var result = await _orderService.GetOrderById(orderId);

        Assert.Equal(expectedOrder, result);
    }

    [Fact]
    public async Task GetOrderById_ThrowsNotFoundException_WhenOrderDoesNotExist()
    {
        var orderId = 1;
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order)null);

        await Assert.ThrowsAsync<OrderNotFoundException>(() => _orderService.GetOrderById(orderId));
    }

    [Fact]
    public async Task CreateOrder_ReturnsNewOrder_WhenGamesExist()
    {
        var userId = 1;
        var gameIds = new List<int> { 1, 2 };
        var createOrderDto = new CreateOrderDto { GameIds = gameIds };
        var user = new User { Id = userId, Country = Countries.US };
        var games = new List<Game> {
            new() { Id = 1, Price = 10, Title = "Game 1", Stock = 12 },
            new() { Id = 2, Price = 20, Title = "Game 2", Stock = 12 }
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(games[0]);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(games[1]);
        _gameBanRepositoryMock.Setup(r => r.GetByCountryAsync(Countries.US))
            .ReturnsAsync(new List<GameBan>());

        var result = await _orderService.CreateOrder(userId, createOrderDto);

        Assert.Equal(userId, result.UserId);
        Assert.Equal(OrderStatus.Created, result.Status);
        Assert.Equal(30, result.TotalSum);
        Assert.Equal(2, result.Games.Count);
        _orderRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_ThrowsUserNotFoundException_WhenUserDoesNotExist() {
        const int userId = 999;
        var createOrderDto = new CreateOrderDto { GameIds = [1] };
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

        await Assert.ThrowsAsync<UserNotFoundException>(() =>
            _orderService.CreateOrder(userId, createOrderDto));
    }

    [Fact]
    public async Task CreateOrder_ThrowsGameNotFoundException_WhenGameDoesNotExist() {
        const int userId = 1;
        var gameIds = new List<int> { 1 };
        var createOrderDto = new CreateOrderDto { GameIds = gameIds };
        var user = new User { Id = userId, Country = Countries.US };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Game)null);

        await Assert.ThrowsAsync<GameNotFoundException>(() =>
            _orderService.CreateOrder(userId, createOrderDto));
    }

    [Fact]
    public async Task CreateOrder_ThrowsBannedGamesException_WhenGameIsBannedInUserCountry() {
        var userId = 1;
        var gameIds = new List<int> { 1 };
        var createOrderDto = new CreateOrderDto { GameIds = gameIds };
        var user = new User { Id = userId, Country = Countries.UA };
        var game = new Game { Id = 1, Price = 10, Title = "Banned Game", Stock = 5 };
        var bannedGame = new GameBan { GameId = 1, Country = Countries.UA };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game);
        _gameBanRepositoryMock.Setup(r => r.GetByCountryAsync(Countries.UA))
            .ReturnsAsync(new List<GameBan> { bannedGame });

        await Assert.ThrowsAsync<BannedGamesInOrderException>(() =>
            _orderService.CreateOrder(userId, createOrderDto));
    }

    [Fact]
    public async Task CreateOrder_ThrowsNoGamesLeftException_WhenGameOutOfStock() {
        var userId = 1;
        var gameIds = new List<int> { 1 };
        var createOrderDto = new CreateOrderDto { GameIds = gameIds };
        var user = new User { Id = userId, Country = Countries.US };
        var game = new Game { Id = 1, Price = 10, Title = "Out of Stock Game", Stock = 0 };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game);
        _gameBanRepositoryMock.Setup(r => r.GetByCountryAsync(Countries.US))
            .ReturnsAsync(new List<GameBan>());

        await Assert.ThrowsAsync<NoGamesLeftException>(() =>
            _orderService.CreateOrder(userId, createOrderDto));
    }

    [Fact]
    public async Task UpdateOrder_ReturnsUpdatedOrder_WhenOrderExists()
    {
        var orderId = 1;
        var newStatus = OrderStatus.Payed;
        var updateOrderDto = new UpdateOrderDto { Status = newStatus };
        var existingOrder = new Order { Id = orderId, Status = OrderStatus.Created, UserId = 1 };
        
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);

        var result = await _orderService.UpdateOrder(orderId, updateOrderDto);

        Assert.Equal(newStatus, result.Status);
        _orderRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrder_WithNewGames_ValidatesGameBans() {
        const int orderId = 1;
        var gameIds = new List<int> { 1 };
        var updateOrderDto = new UpdateOrderDto { Status = OrderStatus.Created, GameIds = gameIds };
        var existingOrder = new Order { Id = orderId, Status = OrderStatus.Created, UserId = 1 };
        var user = new User { Id = 1, Country = Countries.US };
        var game = new Game { Id = 1, Price = 10, Title = "Game 1", Stock = 5 };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game);
        _gameBanRepositoryMock.Setup(r => r.GetByCountryAsync(Countries.US))
            .ReturnsAsync(new List<GameBan>());

        var result = await _orderService.UpdateOrder(orderId, updateOrderDto);

        Assert.Equal(10, result.TotalSum);
        _orderRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrder_WithNewGames_ThrowsNoGamesLeftException_WhenGameOutOfStock() {
        const int orderId = 1;
        var gameIds = new List<int> { 1 };
        var updateOrderDto = new UpdateOrderDto { Status = OrderStatus.Created, GameIds = gameIds };
        var existingOrder = new Order { Id = orderId, Status = OrderStatus.Created, UserId = 1 };
        var user = new User { Id = 1, Country = Countries.US };
        var game = new Game { Id = 1, Price = 10, Title = "Out of Stock Game", Stock = 0 };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game);
        _gameBanRepositoryMock.Setup(r => r.GetByCountryAsync(Countries.US))
            .ReturnsAsync(new List<GameBan>());

        await Assert.ThrowsAsync<NoGamesLeftException>(() =>
            _orderService.UpdateOrder(orderId, updateOrderDto));
    }

    [Fact]
    public async Task DeleteOrder_ReturnsDeletedOrder_WhenOrderExists()
    {
        var orderId = 1;
        var existingOrder = new Order { Id = orderId };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);

        var result = await _orderService.DeleteOrder(orderId);

        Assert.Equal(existingOrder, result);
        _orderRepositoryMock.Verify(r => r.DeleteAsync(existingOrder), Times.Once);
    }

    [Fact]
    public async Task DeleteOrder_ThrowsNotFoundException_WhenOrderDoesNotExist()
    {
        var orderId = 1;
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order)null);

        await Assert.ThrowsAsync<OrderNotFoundException>(() => _orderService.DeleteOrder(orderId));
    }

    [Fact]
    public async Task CreateOrder_DecreasesGameStock_WhenOrderIsCreated()
    {
        var userId = 1;
        var game1 = new Game { Id = 1, Price = 15, Stock = 5 };
        var game2 = new Game { Id = 2, Price = 25, Stock = 8 };
        var createOrderDto = new CreateOrderDto { GameIds = [1, 2] };
        var user = new User { Id = userId, Country = Countries.US };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game1);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(game2);
        _gameBanRepositoryMock.Setup(r => r.GetByCountryAsync(Countries.US))
            .ReturnsAsync(new List<GameBan>());

        await _orderService.CreateOrder(userId, createOrderDto);

        Assert.Equal(4, game1.Stock);
        Assert.Equal(7, game2.Stock);
    }

    [Fact]
    public async Task UpdateOrder_RestoresOldStock_AndDecreasesNewStock()
    {
        var orderId = 1;
        var oldGame = new Game { Id = 1, Price = 10, Stock = 2 };
        var newGame = new Game { Id = 2, Price = 20, Stock = 5 };
        var existingOrder = new Order
        {
            Id = orderId,
            Status = OrderStatus.Created,
            UserId = 1,
            Games = new List<Game> { oldGame }
        };

        var updateDto = new UpdateOrderDto { Status = OrderStatus.Payed, GameIds = [2] };
        var user = new User { Id = 1, Country = Countries.US };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(newGame);
        _gameBanRepositoryMock.Setup(r => r.GetByCountryAsync(Countries.US))
            .ReturnsAsync(new List<GameBan>());

        await _orderService.UpdateOrder(orderId, updateDto);

        Assert.Equal(3, oldGame.Stock);
        Assert.Equal(4, newGame.Stock);
    }

    [Fact]
    public async Task DeleteOrder_RestoresGameStock_WhenOrderDeleted()
    {
        var orderId = 1;
        var game1 = new Game { Id = 1, Price = 15, Stock = 1 };
        var game2 = new Game { Id = 2, Price = 25, Stock = 3 };

        var existingOrder = new Order
        {
            Id = orderId,
            Games = new List<Game> { game1, game2 }
        };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);

        await _orderService.DeleteOrder(orderId);

        Assert.Equal(2, game1.Stock);
        Assert.Equal(4, game2.Stock);
    }
    
    [Fact]
    public async Task ProcessWebhook_UpdatesOrderStatus_WhenOrderExists()
    {
        var orderId = 1;
        var existingOrder = new Order { Id = orderId, Status = OrderStatus.Created };
        var webhookMessage = new WebhookMessage { OrderStatus = OrderStatus.Payed };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);
        
        var result = await _orderService.ProcessWebhook(orderId, webhookMessage);
        
        Assert.Equal(OrderStatus.Payed, result);
        Assert.Equal(OrderStatus.Payed, existingOrder.Status);
        _orderRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Order>(o =>
            o.Id == orderId && o.Status == OrderStatus.Payed)), Times.Once);
    }

    [Fact]
    public async Task ProcessWebhook_CreatesLicense_WhenOrderIsPaid()
    {
        var orderId = 1;
        var existingOrder = new Order { Id = orderId, Status = OrderStatus.Created };
        var webhookMessage = new WebhookMessage { OrderStatus = OrderStatus.Payed };
        
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);
        var result = await _orderService.ProcessWebhook(orderId, webhookMessage);
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Payed, existingOrder.Status);
        _licenseServiceMock.Verify(r => r.CreateLicense(It.IsAny<CreateLicenseDto>()), Times.Once);
    }
    
    [Fact]
    public async Task ProcessWebhook_DoesntCreateLicense_WhenOrderIsCancelled()
    {
        var orderId = 1;
        var existingOrder = new Order { Id = orderId, Status = OrderStatus.Created };
        var webhookMessage = new WebhookMessage { OrderStatus = OrderStatus.Cancelled };
        
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);
        var result = await _orderService.ProcessWebhook(orderId, webhookMessage);
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Cancelled, existingOrder.Status);
        _licenseServiceMock.Verify(r => r.CreateLicense(It.IsAny<CreateLicenseDto>()), Times.Never);
    }
}
