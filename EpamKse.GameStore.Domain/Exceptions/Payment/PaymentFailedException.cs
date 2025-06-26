namespace EpamKse.GameStore.Domain.Exceptions.Payment;

public class PaymentFailedException(string reason) : BadRequestException(reason);