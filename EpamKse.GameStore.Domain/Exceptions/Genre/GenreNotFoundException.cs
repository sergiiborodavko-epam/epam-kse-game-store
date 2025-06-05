namespace EpamKse.GameStore.Domain.Exceptions.Genre;

public class GenreNotFoundException(int id) : NotFoundException($"Genre with ID '{id}' not found.");
