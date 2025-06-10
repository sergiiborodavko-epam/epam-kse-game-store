using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.Domain.DTO.Order;

public class OrdersQueryDto
{
    public int Limit { get; set; } = 10;
    public int Offset { get; set; } = 0;
    public OrderStatus? Status { get; set; }
}