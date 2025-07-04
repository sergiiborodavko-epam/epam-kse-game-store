using EpamKse.GameStore.Domain.DTO.Payments;

namespace EpamKse.GameStore.PaymentService.Services.Payments;

public interface IPaymentService
{
    public Task PayByCreditCard(PaymentInfoCreditCardDto dto);
    public Task PayByIBox(PaymentInfoIBoxDto dto);
    public Task<string> PayByIban(PaymentInfoForIban dto);
}
