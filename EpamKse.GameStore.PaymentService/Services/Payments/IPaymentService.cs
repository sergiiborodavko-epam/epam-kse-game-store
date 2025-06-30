using EpamKse.GameStore.Domain.DTO.Payments;

namespace EpamKse.GameStore.PaymentService.Services.Payments;

public interface IPaymentService
{
    public Task PayByCreditCard(PaymentInfoCreditCardDto dto);
}