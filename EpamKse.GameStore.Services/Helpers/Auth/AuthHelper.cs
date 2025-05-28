using Isopoh.Cryptography.Argon2;

namespace EpamKse.GameStore.Services.Helpers.Auth;

public static class AuthHelper
{
    public static string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }
}