using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; }
    public decimal TotalSum { get; set; } = 0;
    public User User { get; set; }
    public ICollection<Game> Games { get; set; } = new List<Game>();
}