using EpamKse.GameStore.Domain.DTO.Payments;

namespace EpamKse.GameStore.Services.Services.Payment;

using Domain.Enums;

public interface IPaymentService
{
    public Task PayByCreditCard(PayForOrderDto dto);
    public Task PayByIBox(PayForOrderIBoxDto dto);
    Task<OrderStatus> GetPaymentStatus(int orderId);
}
