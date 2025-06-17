using EpamKse.GameStore.DataAccess.Repositories.Game;
using EpamKse.GameStore.DataAccess.Repositories.Order;
using EpamKse.GameStore.Domain.DTO.Order;
using EpamKse.GameStore.Domain.Entities;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions.Game;
using EpamKse.GameStore.Domain.Exceptions.Order;
using EpamKse.GameStore.Services.Services.Order;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace EpamKse.GameStore.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _gameRepositoryMock = new Mock<IGameRepository>();
        _orderService = new OrderService(_orderRepositoryMock.Object, _gameRepositoryMock.Object);
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
    public async Task GetOrders_ReturnsAllOrders_WhenLimitIsValid()
    {
        var queryDto = new OrdersQueryDto { Limit = 10, Offset = 0 };
        var expectedOrders = new List<Order> { new Order() };
        _orderRepositoryMock.Setup(r => r.GetAllAsync(10, 0))
            .ReturnsAsync(expectedOrders);

        var result = await _orderService.GetOrders(queryDto);

        Assert.Equal(expectedOrders, result);
        _orderRepositoryMock.Verify(r => r.GetAllAsync(10, 0), Times.Once);
    }

    [Fact]
    public async Task GetOrders_ReturnsFilteredOrders_WhenStatusProvided()
    {
        var status = OrderStatus.Created;
        var queryDto = new OrdersQueryDto { Status = status, Limit = 10, Offset = 0 };
        var expectedOrders = new List<Order> { new Order() };
        _orderRepositoryMock.Setup(r => r.GetByStatusAsync(status))
            .ReturnsAsync(expectedOrders);

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
        var games = new List<Game>
        {
            new Game { Id = 1, Price = 10,Stock = 12 },
            new Game { Id = 2, Price = 20,Stock = 12 }
        };

        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(games[0]);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(games[1]);

        var result = await _orderService.CreateOrder(userId, createOrderDto);

        Assert.Equal(userId, result.UserId);
        Assert.Equal(OrderStatus.Created, result.Status);
        Assert.Equal(30, result.TotalSum);
        Assert.Equal(2, result.Games.Count);
        _orderRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_ThrowsGameNotFoundException_WhenGameDoesNotExist()
    {
        var userId = 1;
        var gameIds = new List<int> { 1 };
        var createOrderDto = new CreateOrderDto { GameIds = gameIds };
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Game)null);

        await Assert.ThrowsAsync<GameNotFoundException>(() => 
            _orderService.CreateOrder(userId, createOrderDto));
    }

    [Fact]
    public async Task UpdateOrder_ReturnsUpdatedOrder_WhenOrderExists()
    {
        var orderId = 1;
        var newStatus = OrderStatus.Payed;
        var updateOrderDto = new UpdateOrderDto { Status = newStatus };
        var existingOrder = new Order { Id = orderId, Status = OrderStatus.Created };
        
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);

        var result = await _orderService.UpdateOrder(orderId, updateOrderDto);

        Assert.Equal(newStatus, result.Status);
        _orderRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrder_ThrowsNotFoundException_WhenOrderDoesNotExist()
    {
        var orderId = 1;
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order)null);

        await Assert.ThrowsAsync<OrderNotFoundException>(() => 
            _orderService.UpdateOrder(orderId, new UpdateOrderDto { Status = OrderStatus.Created }));
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
}