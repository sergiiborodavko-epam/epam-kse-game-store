namespace EpamKse.GameStore.Domain.Exceptions;

public class GameNotFoundException(int id) : Exception($"Game with ID {id} not found");

public class GameAlreadyExistsException(string title) : Exception($"Game with title '{title}' already exists");
