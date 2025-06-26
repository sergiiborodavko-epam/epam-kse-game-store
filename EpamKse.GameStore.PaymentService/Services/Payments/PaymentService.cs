using EpamKse.GameStore.Domain.DTO.Payments;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Domain.Exceptions.Payment;

namespace EpamKse.GameStore.PaymentService.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task PayByCreditCard(PaymentInfoCreditCardDto dto)
    {
        if (IsCardExpired(dto.ExpirationDate))
        {
            throw new PaymentFailedException("Card expired");
        }

        var apiClient = _httpClientFactory.CreateClient("ApiClient");
        var paymentResult = await TryPayTheOrder();
        var orderStatus = paymentResult ? OrderStatus.Payed : OrderStatus.Cancelled;
        
        await apiClient.PostAsJsonAsync(dto.CallbackUrl, new { orderStatus = orderStatus });
        
        if (!paymentResult)
        {
            throw new PaymentFailedException("Insufficient funds");
        }
    }

    private Task<bool> TryPayTheOrder()
    {
        const int probabilityOfSuccess = 50;
        var random = new Random();
        var result = random.Next(0, 100) < probabilityOfSuccess;
        return Task.FromResult(result);
    }
    private bool IsCardExpired(string expirationDate)
    {
        var parts = expirationDate.Split('/');
        if (parts.Length != 2)
        {
            return true;
        }

        if (!int.TryParse(parts[0], out var month) || !int.TryParse(parts[1], out var year))
        {
            return true;
        }

        if (month < 1 || month > 12)
        {
            return true;
        }

        var century = 2000;
        var fullYear = century + year;
        
        var expDate = new DateTime(fullYear, month, 1);
        
        return expDate < DateTime.UtcNow;
    }
}