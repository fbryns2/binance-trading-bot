using System.Security.Cryptography;

namespace BinBot.Cryptography.Codecs;

public static class Sha256Hasher
{
    public static byte[] Hash(ReadOnlySpan<byte> data) => SHA256.HashData(data);

    public static byte[] HashTwice(ReadOnlySpan<byte> data)
    {
        var first = SHA256.HashData(data);
        return SHA256.HashData(first);
    }

    public static string HashToHex(ReadOnlySpan<byte> data) =>
        Convert.ToHexString(Hash(data)).ToLowerInvariant();
}
