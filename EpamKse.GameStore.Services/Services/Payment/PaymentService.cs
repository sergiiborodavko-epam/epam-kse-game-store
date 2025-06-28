using System.Net.Http.Json;
using System.Text.Json;
using EpamKse.GameStore.DataAccess.Repositories.Order;
using EpamKse.GameStore.Domain.DTO.License;
using EpamKse.GameStore.Domain.DTO.Payments;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions.Order;
using EpamKse.GameStore.Domain.Exceptions.Payment;
using EpamKse.GameStore.Services.Services.License;

namespace EpamKse.GameStore.Services.Services.Payment;

public class PaymentService : IPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOrderRepository _orderRepository;

    public PaymentService(IHttpClientFactory httpClientFactory, IOrderRepository orderRepository)
    {
        _httpClientFactory = httpClientFactory;
        _orderRepository = orderRepository;
    }
    
    public async Task PayByCreditCard(PayForOrderDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(dto.OrderId);
        if (order == null)
        {
            throw new OrderNotFoundException(dto.OrderId);
        }

        if (order.Status == OrderStatus.Payed)
        {
            throw new OrderAlreadyPaidException(order.Id);
        }

        order.Status = OrderStatus.Initiated;
        await _orderRepository.UpdateAsync(order);

        var paymentServiceClient = _httpClientFactory.CreateClient("PaymentServiceClient");
        paymentServiceClient.PostAsJsonAsync("payments/credit-card", new PaymentInfoCreditCardDto
        {
            CardNumber = dto.CardNumber,
            Cvv = dto.Cvv,
            ExpirationMonth = dto.ExpirationMonth,
            ExpirationYear = dto.ExpirationYear,
            OrderId = order.Id,
            TotalSum = order.TotalSum,
            CallbackUrl = $"/orders/orderWebhook/{order.Id}"
        });
    }

    public async Task<string> PayByIban(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new OrderNotFoundException(orderId);
        }
        if (order.Status == OrderStatus.Payed)
        {
            throw new OrderAlreadyPaidException(order.Id);
        }
        order.Status = OrderStatus.Initiated;
        await _orderRepository.UpdateAsync(order);
        var paymentServiceClient = _httpClientFactory.CreateClient("PaymentServiceClient");
       var result= await paymentServiceClient.PostAsJsonAsync("payments/iban", new PaymentInfoForIban
        {
            OrderId = order.Id,
            TotalSum = order.TotalSum,
            CallbackUrl = $"/orders/orderWebhook/{order.Id}"
        });
        result.EnsureSuccessStatusCode();
        var ibanResult = await result.Content.ReadFromJsonAsync<IbanResultDto>();
        return ibanResult?.Iban ?? throw new InvalidOperationException("IBAN not returned from payment service.");
    }
}