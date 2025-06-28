using EpamKse.GameStore.Domain.DTO.Payments;

namespace EpamKse.GameStore.Services.Services.Payment;

public interface IPaymentService
{
    public Task PayByCreditCard(PayForOrderDto dto);
    public Task<string> PayByIban(int orderId);
}