namespace EpamKse.GameStore.Domain.Exceptions.Game;

public class GameAlreadyExistsException(string title) : ConflictException($"Game with title '{title}' already exists");
public class GameNotFoundException(int id) : NotFoundException($"Game with ID '{id}' not found");
