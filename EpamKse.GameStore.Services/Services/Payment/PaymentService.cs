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
    private readonly ILicenseService _licenseService;
    private readonly IOrderRepository _orderRepository;

    public PaymentService(IHttpClientFactory httpClientFactory, ILicenseService licenseService,
        IOrderRepository orderRepository)
    {
        _httpClientFactory = httpClientFactory;
        _licenseService = licenseService;
        _orderRepository = orderRepository;
    }
    
    public async Task<byte[]> PayByCreditCard(PayForOrderDto dto)
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
        var result = await paymentServiceClient.PostAsJsonAsync("payments/credit-card", new PaymentInfoCreditCardDto
        {
            CardNumber = dto.CardNumber,
            Cvv = dto.Cvv,
            ExpirationMonth = dto.ExpirationMonth,
            ExpirationYear = dto.ExpirationYear,
            OrderId = order.Id,
            TotalSum = order.TotalSum,
            CallbackUrl = $"/orders/orderWebhook/{order.Id}"
        });
        
        if (!result.IsSuccessStatusCode)
        {
            var jsonString = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<PaymentResultDto>(jsonString);
            throw new PaymentFailedException(response.message);
        }

        await _licenseService.CreateLicense(new CreateLicenseDto { OrderId = order.Id });
        var license = await _licenseService.GenerateLicenseFileByOrderId(order.Id);
        return license;
    }
}