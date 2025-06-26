namespace EpamKse.GameStore.Domain.Exceptions.Order;

public class OrderAlreadyPaidException(int id) : ConflictException($"Order with id {id} is already paid for");