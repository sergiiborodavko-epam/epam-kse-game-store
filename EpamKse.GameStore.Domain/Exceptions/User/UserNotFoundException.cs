namespace EpamKse.GameStore.Domain.Exceptions.User;

public class UserNotFoundException(int id) : NotFoundException($"User with id {id} was not found") {}