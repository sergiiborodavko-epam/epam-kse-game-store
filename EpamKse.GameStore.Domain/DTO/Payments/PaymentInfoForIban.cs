using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Domain.DTO.Payments;

public class PaymentInfoForIban
{
    
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }
    
    [Required]
    public string CallbackUrl { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public decimal TotalSum { get; set; }
}