namespace EpamKse.GameStore.Api.Exceptions.Auth;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException() : base("User already exists.")
    {
    }
}