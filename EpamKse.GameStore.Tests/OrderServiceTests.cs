using EpamKse.GameStore.DataAccess.Repositories.Game;
using EpamKse.GameStore.DataAccess.Repositories.Order;
using EpamKse.GameStore.Domain.DTO.Order;
using EpamKse.GameStore.Domain.Entities;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions.Game;
using EpamKse.GameStore.Domain.Exceptions.Order;
using EpamKse.GameStore.Services.Services.Order;
using Moq;

namespace EpamKse.GameStore.Tests;

[TestFixture]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IGameRepository> _gameRepositoryMock;
    private OrderService _orderService;

    [SetUp]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _gameRepositoryMock = new Mock<IGameRepository>();
        _orderService = new OrderService(_orderRepositoryMock.Object, _gameRepositoryMock.Object);
    }

    [Test]
    public async Task GetOrders_ReturnsAllOrders()
    {
        var queryDto = new OrdersQueryDto { Limit = 10, Offset = 0 };
        var expectedOrders = new List<Order> { new Order() };
        _orderRepositoryMock.Setup(r => r.GetAllAsync(queryDto.Limit, queryDto.Offset))
            .ReturnsAsync(expectedOrders);

        var result = await _orderService.GetOrders(queryDto);

        Assert.That(result, Is.EqualTo(expectedOrders));
        _orderRepositoryMock.Verify(r => r.GetAllAsync(queryDto.Limit, queryDto.Offset), Times.Once);
    }

    [Test]
    public async Task GetOrders_ReturnsFilteredOrders_WhenStatusProvided() 
    {
        var status = OrderStatus.Created;
        var queryDto = new OrdersQueryDto { Status = status, Limit = 10, Offset = 0 };
        var expectedOrders = new List<Order> { new Order() };
        _orderRepositoryMock.Setup(r => r.GetByStatusAsync(status, queryDto.Limit, queryDto.Offset))
            .ReturnsAsync(expectedOrders);

        var result = await _orderService.GetOrders(queryDto);

        Assert.That(result, Is.EqualTo(expectedOrders));
        _orderRepositoryMock.Verify(r => r.GetByStatusAsync(status, queryDto.Limit, queryDto.Offset), Times.Once);
    }

    [Test]
    public async Task GetOrderById_ReturnsOrder_WhenOrderExists() 
    {
        var orderId = 1;
        var expectedOrder = new Order { Id = orderId };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(expectedOrder);

        var result = await _orderService.GetOrderById(orderId);

        Assert.That(result, Is.EqualTo(expectedOrder));
    }

    [Test]
    public void GetOrderById_ThrowsNotFoundException_WhenOrderDoesNotExist() 
    {
        var orderId = 1;
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order)null);

        Assert.ThrowsAsync<OrderNotFoundException>(async () => await _orderService.GetOrderById(orderId));
    }

    [Test]
    public async Task CreateOrder_ReturnsNewOrder_WhenGamesExist()
    {
        var userId = 1;
        var gameIds = new List<int> { 1, 2 };
        var createOrderDto = new CreateOrderDto { GameIds = gameIds };
        var games = new List<Game>
        {
            new Game { Id = 1, Price = 10 },
            new Game { Id = 2, Price = 20 }
        };

        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(games[0]);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(games[1]);

        var result = await _orderService.CreateOrder(userId, createOrderDto);

        Assert.That(result.UserId, Is.EqualTo(userId));
        Assert.That(result.Status, Is.EqualTo(OrderStatus.Created));
        Assert.That(result.Price, Is.EqualTo(30));
        Assert.That(result.Games.Count, Is.EqualTo(2));
        _orderRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Test]
    public void CreateOrder_ThrowsGameNotFoundException_WhenGameDoesNotExist()
    {
        var userId = 1;
        var gameIds = new List<int> { 1 };
        var createOrderDto = new CreateOrderDto { GameIds = gameIds };
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Game)null);

        Assert.ThrowsAsync<GameNotFoundException>(async () => 
            await _orderService.CreateOrder(userId, createOrderDto));
    }

    [Test]
    public async Task UpdateOrder_ReturnsUpdatedOrder_WhenOrderExists()
    {
        var orderId = 1;
        var newStatus = OrderStatus.Payed;
        var updateOrderDto = new UpdateOrderDto { Status = newStatus };
        var existingOrder = new Order { Id = orderId, Status = OrderStatus.Created };
        
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);

        var result = await _orderService.UpdateOrder(orderId, updateOrderDto);

        Assert.That(result.Status, Is.EqualTo(newStatus));
        _orderRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Test]
    public void UpdateOrder_ThrowsNotFoundException_WhenOrderDoesNotExist() 
    {
        var orderId = 1;
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order)null);

        Assert.ThrowsAsync<OrderNotFoundException>(async () => 
            await _orderService.UpdateOrder(orderId, new UpdateOrderDto { Status = OrderStatus.Created }));
    }

    [Test]
    public async Task DeleteOrder_ReturnsDeletedOrder_WhenOrderExists() 
    {
        var orderId = 1;
        var existingOrder = new Order { Id = orderId };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(existingOrder);

        var result = await _orderService.DeleteOrder(orderId);

        Assert.That(result, Is.EqualTo(existingOrder));
        _orderRepositoryMock.Verify(r => r.DeleteAsync(existingOrder), Times.Once);
    }

    [Test]
    public void DeleteOrder_ThrowsNotFoundException_WhenOrderDoesNotExist() 
    {
        var orderId = 1;
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order)null);

        Assert.ThrowsAsync<OrderNotFoundException>(async () => await _orderService.DeleteOrder(orderId));
    }
}