using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Domain.DTO.Payments;

public class PaymentInfoIBoxDto {
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public decimal TotalSum { get; set; }
    
    [Required]
    public string CallbackUrl { get; set; }
}
