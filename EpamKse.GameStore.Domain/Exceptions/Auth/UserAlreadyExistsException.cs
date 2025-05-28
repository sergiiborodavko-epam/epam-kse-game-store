namespace EpamKse.GameStore.Domain.Exceptions.Auth;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException() : base("User already exists.")
    {
    }
}