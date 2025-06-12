using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.Domain.DTO.Order;

public class OrdersQueryDto
{
    public int? Limit { get; set; }
    public int Offset { get; set; } = 0;
    public OrderStatus? Status { get; set; }
}