namespace EpamKse.GameStore.Domain.Exceptions.GameBan;

public class BanNotFoundException(int id) : NotFoundException($"Ban with ID {id} not found");
