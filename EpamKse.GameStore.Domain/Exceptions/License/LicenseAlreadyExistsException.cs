namespace EpamKse.GameStore.Domain.Exceptions.License;

public class LicenseAlreadyExistsException(int id) : ConflictException($"License for order id {id} already exists") {}