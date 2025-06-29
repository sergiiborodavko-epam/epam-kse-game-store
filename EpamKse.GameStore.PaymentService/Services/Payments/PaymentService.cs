using EpamKse.GameStore.Domain.DTO.Payments;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions.Payment;

namespace EpamKse.GameStore.PaymentService.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string IBAN;
    public PaymentService(IHttpClientFactory httpClientFactory)
    {
        IBAN=Environment.GetEnvironmentVariable("IBAN");
        _httpClientFactory = httpClientFactory;
    }

    public async Task PayByCreditCard(PaymentInfoCreditCardDto dto)
    {
        if (IsCardExpired(dto.ExpirationMonth, dto.ExpirationYear))
        {
            throw new PaymentFailedException("Card expired");
        }

        var apiClient = _httpClientFactory.CreateClient("ApiClient");
        var paymentResult = await TryPayTheOrder();
        var orderStatus = paymentResult ? OrderStatus.Payed : OrderStatus.Cancelled;
        
        apiClient.PostAsJsonAsync(dto.CallbackUrl, new { orderStatus = orderStatus });
    }
    public async Task<string> PayByIban(PaymentInfoForIban dto)
    {
        var apiClient = _httpClientFactory.CreateClient("ApiClient");
        var paymentResult = await TryPayTheOrder();
        var orderStatus = paymentResult ? OrderStatus.Payed : OrderStatus.Cancelled;
        await apiClient.PostAsJsonAsync(dto.CallbackUrl, new { orderStatus = orderStatus });
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