using EpamKse.GameStore.Domain.DTO.Payments;
using EpamKse.GameStore.PaymentService.Services.Payments;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.PaymentService.Controllers;

[ApiController]
[Route("payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    
    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost("credit-card")]
    public async Task<IActionResult> PayByCreditCard([FromBody] PaymentInfoCreditCardDto dto)
    {
        await _paymentService.PayByCreditCard(dto);
        return Ok(new { message = "Payment created"});
    }
}