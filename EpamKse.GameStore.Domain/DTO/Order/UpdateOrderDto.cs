using System.ComponentModel.DataAnnotations;
using EpamKse.GameStore.Domain.Enums;

namespace EpamKse.GameStore.Domain.DTO.Order;

public class UpdateOrderDto
{
    [Required]
    public OrderStatus? Status { get; set; }
    public List<int>? GameIds { get; set; }
}