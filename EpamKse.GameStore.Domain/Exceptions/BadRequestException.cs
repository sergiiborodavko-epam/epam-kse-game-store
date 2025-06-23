namespace EpamKse.GameStore.Domain.Exceptions;

public class BadRequestException(string message) : CustomHttpException(400, message);