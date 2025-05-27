namespace EpamKse.GameStore.Domain.Exceptions;

public class ConflictException(string message) : CustomHttpException(409, message);