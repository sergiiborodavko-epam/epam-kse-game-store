namespace EpamKse.GameStore.Services.Helpers.Payment;

public static class CallBackUrlBuilder
{
    public static string GetCallBackUrl(int orderId)
    {
        return $"/orders/orderWebhook/{orderId}";
    }
}