namespace EpamKse.GameStore.Domain.Exceptions.Game;

public class GameNotFoundException(int id) : NotFoundException($"Game with ID '{id}' not found");
