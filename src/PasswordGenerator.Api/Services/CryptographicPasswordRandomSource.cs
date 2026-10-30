using System.Security.Cryptography;

namespace PasswordGenerator.Api.Services;

public sealed class CryptographicPasswordRandomSource : IPasswordRandomSource
{
    public int GetInt32(int exclusiveUpperBound)
    {
        return RandomNumberGenerator.GetInt32(exclusiveUpperBound);
    }
}