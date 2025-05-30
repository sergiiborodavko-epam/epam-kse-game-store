namespace EpamKse.GameStore.Domain.Exceptions.User;

public class UserNotFoundException(string message) : NotFoundException(message) {}