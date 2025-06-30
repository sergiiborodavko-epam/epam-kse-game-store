using System.Net.Http.Json;
using EpamKse.GameStore.Services.Helpers.Payment;

namespace EpamKse.GameStore.Services.Services.Payment;

using DataAccess.Repositories.Order;
using DataAccess.Repositories.User;
using Domain.DTO.Payments;
using Domain.Enums;
using Domain.Exceptions.Order;
using Domain.Exceptions.User;
using Domain.Entities;

public class PaymentService(IHttpClientFactory httpClientFactory, IOrderRepository orderRepository, IUserRepository userRepository) : IPaymentService {
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("PaymentServiceClient");
    
    public async Task PayByCreditCard(PayForOrderDto dto) {
        var order = await CheckOrderStatusAsync(dto.OrderId);

        await _httpClient.PostAsJsonAsync("payments/credit-card", new PaymentInfoCreditCardDto {
            CardNumber = dto.CardNumber,
            Cvv = dto.Cvv,
            ExpirationMonth = dto.ExpirationMonth,
            ExpirationYear = dto.ExpirationYear,
            OrderId = order.Id,
            TotalSum = order.TotalSum,
            CallbackUrl = CallBackUrlBuilder.GetCallBackUrl(order.Id)
        });
    }
    public async Task PayByIBox(PayForOrderIBoxDto dto) {
        var order = await CheckOrderStatusAsync(dto.OrderId);
        
        var user = await userRepository.GetByIdAsync(dto.UserId);
        if (user == null) {
            throw new UserNotFoundException(dto.UserId);
        }

        await _httpClient.PostAsJsonAsync("payments/ibox", new PaymentInfoIBoxDto {
            OrderId = order.Id,
            UserId = dto.UserId,
            TotalSum = order.TotalSum,
            CallbackUrl = CallBackUrlBuilder.GetCallBackUrl(order.Id)
        });
    }

    public async Task<string> PayByIban(PayForOrderIbanDto dto) {
        var order = await CheckOrderStatusAsync(dto.OrderId);
        
        var result = await _httpClient.PostAsJsonAsync("payments/iban", new PaymentInfoForIban {
            OrderId = order.Id,
            TotalSum = order.TotalSum,
            CallbackUrl = CallBackUrlBuilder.GetCallBackUrl(order.Id)
        });
        
        result.EnsureSuccessStatusCode();
        var ibanResult = await result.Content.ReadFromJsonAsync<IbanResultDto>();
        return ibanResult?.Iban ?? throw new InvalidOperationException("IBAN not returned from payment service.");
    }
    
    public async Task<OrderStatus> GetPaymentStatus(int orderId) {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null) {
            throw new OrderNotFoundException(orderId);
        }
        return order.Status;
    }
    
    private async Task<Order> CheckOrderStatusAsync(int orderId) {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null) {
            throw new OrderNotFoundException(orderId);
        }

        if (order.Status == OrderStatus.Payed) {
            throw new OrderAlreadyPaidException(order.Id);
        }
        
        order.Status = OrderStatus.Initiated;
        await orderRepository.UpdateAsync(order);
        return order;
    }
}
