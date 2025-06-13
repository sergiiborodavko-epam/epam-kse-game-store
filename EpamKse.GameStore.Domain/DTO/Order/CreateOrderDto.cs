using System.ComponentModel.DataAnnotations;
using EpamKse.GameStore.Domain.Entities;

namespace EpamKse.GameStore.Domain.DTO.Order;

public class CreateOrderDto
{
    public List<int> GameIds { get; set; }
}