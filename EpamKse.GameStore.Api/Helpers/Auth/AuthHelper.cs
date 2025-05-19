using Isopoh.Cryptography.Argon2;

namespace EpamKse.GameStore.Api.Helpers.Auth;

public static class AuthHelper
{
    public static string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }
}