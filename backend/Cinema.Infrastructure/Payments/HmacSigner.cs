using System.Security.Cryptography;
using System.Text;

namespace Cinema.Infrastructure.Payments;

internal static class HmacSigner
{
    public static string Sha256Hex(string key, string data) =>
        ToLowerHex(HMACSHA256.HashData(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(data)));

    public static string Sha512Hex(string key, string data) =>
        ToLowerHex(HMACSHA512.HashData(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(data)));

    private static string ToLowerHex(byte[] hash) => Convert.ToHexString(hash).ToLowerInvariant();
}
