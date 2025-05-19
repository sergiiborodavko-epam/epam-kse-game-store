namespace EpamKse.GameStore.Api.Exceptions;

public class ConflictException(string message) : CustomHttpException(409, message);