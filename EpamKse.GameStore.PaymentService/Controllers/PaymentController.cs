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
    
    [HttpPost("ibox")]
    public async Task<IActionResult> PayByIBox([FromBody] PaymentInfoIBoxDto dto) {
        await _paymentService.PayByIBox(dto);
        return Ok(new { message = "Payment created", dynamicUrl = dto.DynamicUrl });
    }
    [HttpPost("iban")]
    public async Task<IActionResult> PayByIban([FromBody] PaymentInfoForIban dto)
    {
       var IBAN= await _paymentService.PayByIban(dto);
       return Ok(new { iban = IBAN });
    }
}
