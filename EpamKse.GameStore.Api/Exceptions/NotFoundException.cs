namespace EpamKse.GameStore.Api.Exceptions;

public class NotFoundException(string message) : CustomHttpException(404, message);