using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.Domain.DTO.Order;

public class WebhookMessage
{
    public OrderStatus OrderStatus { get; set; }
}