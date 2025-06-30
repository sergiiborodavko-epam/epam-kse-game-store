using EpamKse.GameStore.Domain.DTO.Payments;
using EpamKse.GameStore.Services.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Authorize(Policy = "UserPolicy")]
[Route("payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost("credit-card")]
    public async Task PayByCreditCard([FromBody] PayForOrderDto dto)
    {
        await _paymentService.PayByCreditCard(dto);
    }
    
    [HttpPost("ibox")]
    public async Task PayByIBox([FromBody] PayForOrderIBoxDto dto) {
        await _paymentService.PayByIBox(dto);
    }
}
