using System.Security.Cryptography;
using System.Text;

namespace EpamKse.GameStore.Services.Services.Encryption;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    
    public EncryptionService()
    {
        var keyString = Environment.GetEnvironmentVariable("LICENSE_ENCRYPTION_KEY");
        _key = Encoding.ASCII.GetBytes(keyString);
    }
    
    public string Encrypt(string value)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.GenerateIV();
            
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(value);
                    }
                }

                var payload = Convert.ToBase64String(msEncrypt.ToArray());
                var ivString = Convert.ToBase64String(aesAlg.IV);
                return $"{payload}.{ivString}";
            }
        }
    }

    public string Decrypt(string value)
    {
        var parts = value.Split('.');
        if (parts.Length != 2)
        {
            throw new FormatException();
        }
        
        var payload = parts[0];
        var iv = Convert.FromBase64String(parts[1]);
        
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = iv;
            var payloadBytes = Convert.FromBase64String(payload);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msDecrypt = new MemoryStream(payloadBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}