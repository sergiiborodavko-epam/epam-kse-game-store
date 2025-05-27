namespace EpamKse.GameStore.Domain.Exceptions;

public class NotFoundException(string message) : CustomHttpException(404, message);