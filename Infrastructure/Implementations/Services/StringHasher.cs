using Application.Abstractions.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Implementations.Services;

internal class StringHasher : IStringHasher
{
    public string Hash(string plainText)
        => Convert.ToBase64String(SHA3_512.HashData(Encoding.UTF8.GetBytes(plainText)));
}
