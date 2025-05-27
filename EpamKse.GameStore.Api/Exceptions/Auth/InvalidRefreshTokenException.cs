namespace EpamKse.GameStore.Api.Exceptions.Auth;

public class InvalidRefreshTokenException:Exception
{
    public InvalidRefreshTokenException() : base("Invalid refresh token.")
    {
    }
}