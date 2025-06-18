using EpamKse.GameStore.Domain.DTO.Game;
using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.Domain.DTO.Order;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; }
    public decimal TotalSum { get; set; }
    public List<GameViewDto> Games { get; set; }
}