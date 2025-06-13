namespace EpamKse.GameStore.Domain.Exceptions.GameFile;

public class GameFileNotFoundException(int id) : NotFoundException($"Game file with ID {id} not found");
