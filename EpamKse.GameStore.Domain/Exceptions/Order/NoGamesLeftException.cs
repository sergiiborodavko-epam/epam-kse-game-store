namespace EpamKse.GameStore.Domain.Exceptions.Order;

public class NoGamesLeftException(int id) : ConflictException($"No game copies left for game with id {id}")
{
}