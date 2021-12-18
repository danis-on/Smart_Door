using OtpNet;
using SmartDoor.Server.Models.Database;
using System.Security.Cryptography;
using System.Text;

namespace SmartDoor.Server;

internal static class UserHelper
{
    private static string BytesToHex(byte[] arr)
    {
        var builder = new StringBuilder();
        foreach (var b in arr)
            builder.Append(b.ToString("x2"));
        return builder.ToString();
    }

    internal static string GetPasswordHash(string login, string password)
    {
        using var sha = SHA256.Create();
        return BytesToHex(sha.ComputeHash(Encoding.UTF8.GetBytes($"{login}:{password}")));
    }

    internal static bool ValidateRole(UserRole role, bool allowAnonymous = false)
    {
        if (!allowAnonymous && role == UserRole.Anonymous)
            return false;

        var possible = Enum.GetValues<UserRole>();
        foreach (var p in possible)
            if (role.HasFlag(p))
                role ^= p;
        if (role != UserRole.Anonymous)
            return false;

        return true;
    }
}

