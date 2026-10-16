using System.Security.Cryptography;
using System.Text;

namespace Api.WhatsApp.Services;

public static class WhatsAppSignature
{
    /// <summary>
    /// Valida o header X-Hub-Signature-256 enviado pela Meta (HMAC-SHA256 do corpo cru
    /// usando o App Secret). Se o AppSecret estiver vazio (dev), a validacao e ignorada.
    /// </summary>
    public static bool IsValid(byte[] rawBody, string? header, string appSecret)
    {
        if (string.IsNullOrEmpty(appSecret)) return true;
        if (string.IsNullOrEmpty(header) || !header.StartsWith("sha256=")) return false;

        var provided = header["sha256=".Length..].ToLowerInvariant();

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
        var computed = Convert.ToHexString(hmac.ComputeHash(rawBody)).ToLowerInvariant();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(computed),
            Encoding.ASCII.GetBytes(provided));
    }
}
