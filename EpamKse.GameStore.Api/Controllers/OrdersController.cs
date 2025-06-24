using AutoMapper;
using EpamKse.GameStore.Domain.DTO.Order;
using EpamKse.GameStore.Services.Services.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Route("orders")]
[Authorize(Policy = "UserPolicy")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;

    public OrdersController(IOrderService orderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] OrdersQueryDto queryDto)
    {
        var orders = await _orderService.GetOrders(queryDto);
        return Ok(_mapper.Map<List<OrderDto>>(orders));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderById(id);
        return Ok(_mapper.Map<OrderDto>(order));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto createOrderDto)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")!.Value;
        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        var userId = int.Parse(userIdClaim);
        var order = await _orderService.CreateOrder(userId, createOrderDto);
        return Created($"/orders/${order.Id}", _mapper.Map<OrderDto>(order));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto updateOrderDto)
    {
        var order = await _orderService.UpdateOrder(id, updateOrderDto);
        return Ok(_mapper.Map<OrderDto>(order));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _orderService.DeleteOrder(id);
        return Ok(_mapper.Map<OrderDto>(order));
    }

    [HttpPatch("orderWebhook/{id:int}")]
    public async Task<IActionResult> OrderWebhook(int id, WebhookMessage webhookMessage)
    {
        var result = await _orderService.ProcessWebhook(id, webhookMessage);
        return Ok(result);
    }
}