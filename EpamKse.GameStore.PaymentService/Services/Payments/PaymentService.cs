using EpamKse.GameStore.Domain.DTO.Payments;
using EpamKse.GameStore.Domain.DTO.Order;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions.Payment;

namespace EpamKse.GameStore.PaymentService.Services.Payments;

public class PaymentService(IHttpClientFactory httpClientFactory) : IPaymentService {
    private readonly HttpClient _apiClient = httpClientFactory.CreateClient("ApiClient");
    private readonly string IBAN = Environment.GetEnvironmentVariable("IBAN");

    public async Task PayByCreditCard(PaymentInfoCreditCardDto dto)
    {
        if (IsCardExpired(dto.ExpirationMonth, dto.ExpirationYear))
        {
            throw new PaymentFailedException("Card expired");
        }

        var paymentResult = await TryPayTheOrder();
        var orderStatus = paymentResult ? OrderStatus.Payed : OrderStatus.Cancelled;
        
        await _apiClient.PostAsJsonAsync(dto.CallbackUrl, new WebhookMessage { OrderStatus = orderStatus });
    }
    
    public async Task PayByIBox(PaymentInfoIBoxDto dto) {
        var paymentResult = await TryPayTheOrder();
        var orderStatus = paymentResult ? OrderStatus.Payed : OrderStatus.Cancelled;
        
        await _apiClient.PostAsJsonAsync(dto.CallbackUrl, new WebhookMessage { OrderStatus = orderStatus });
    }
    
    public async Task<string> PayByIban(PaymentInfoForIban dto) {
        var paymentResult = await TryPayTheOrder();
        var orderStatus = paymentResult ? OrderStatus.Payed : OrderStatus.Cancelled;
        await _apiClient.PostAsJsonAsync(dto.CallbackUrl, new WebhookMessage { OrderStatus = orderStatus });
        return IBAN;
    }

    private Task<bool> TryPayTheOrder()
    {
        const int probabilityOfSuccess = 50;
        var random = new Random();
        var result = random.Next(0, 100) < probabilityOfSuccess;
        return Task.FromResult(result);
    }
    
    private bool IsCardExpired(int month, int year)
    {
        var expDate = new DateTime(year, month, 1);
        return expDate < DateTime.UtcNow;
    }
}
