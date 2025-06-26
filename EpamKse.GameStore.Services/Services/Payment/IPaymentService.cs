using EpamKse.GameStore.Domain.DTO.Payments;

namespace EpamKse.GameStore.Services.Services.Payment;

public interface IPaymentService
{
    public Task<byte[]> PayByCreditCard(PayForOrderDto dto);
}