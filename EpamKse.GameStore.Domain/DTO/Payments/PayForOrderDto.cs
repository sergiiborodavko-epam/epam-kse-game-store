using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Domain.DTO.Payments;

public class PayForOrderDto
{
    [Required]
    [CreditCard]
    public string CardNumber { get; set; }
    
    [Required]
    [Length(5, 5)]
    public string ExpirationDate { get; set; }
    
    [Required]
    [Range(100, 999)]
    public int Cvv { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }
}