using System.Text;
using System.Security.Cryptography;

namespace HikCentral.Utils;
public static class Encrypt
{
    private static readonly Encoding encoding = Encoding.UTF8;
    public static string HikCentralSignature(string? secret, string message)
    {
        if (secret is null)
            throw new ArgumentException("No se proporcionó el SecretKey al momento de generar la signature.");
        var secretByte = encoding.GetBytes(secret);
        using var hmacsha256 = new HMACSHA256(secretByte);
        hmacsha256.ComputeHash(encoding.GetBytes(message));
        if (hmacsha256.Hash is null)
            throw new InvalidOperationException("Hash computation failed.");
        return Convert.ToBase64String(hmacsha256.Hash);
    }
}
