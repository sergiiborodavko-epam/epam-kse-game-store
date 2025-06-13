namespace EpamKse.GameStore.Domain.Exceptions.Order;

public class OrderNotFoundException(int id) : NotFoundException($"Order with id {id} was not found") {}