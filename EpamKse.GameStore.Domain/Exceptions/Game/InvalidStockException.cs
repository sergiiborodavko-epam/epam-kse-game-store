namespace EpamKse.GameStore.Domain.Exceptions.Game;

public class InvalidStockException(): ConflictException("Stock must be at least 1");