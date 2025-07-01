using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Domain.DTO.Payments;

public class PayForOrderIBoxDto {
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }
}
