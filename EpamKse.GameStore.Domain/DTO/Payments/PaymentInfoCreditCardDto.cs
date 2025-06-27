using System.ComponentModel.DataAnnotations;

namespace EpamKse.GameStore.Domain.DTO.Payments;

public class PaymentInfoCreditCardDto
{
    [Required]
    [CreditCard]
    public string CardNumber { get; set; }
    
    [Required]
    [Range(1, 12)]
    public int ExpirationMonth { get; set; }
    
    [Required]
    [Range(2000, 2100)]
    public int ExpirationYear { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public decimal TotalSum { get; set; }
    
    [Required]
    [Range(100, 999)]
    public int Cvv { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int OrderId { get; set; }
    
    [Required]
    public string CallbackUrl { get; set; }
}