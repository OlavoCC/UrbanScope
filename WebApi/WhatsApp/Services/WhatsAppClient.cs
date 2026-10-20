using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Api.WhatsApp.Config;
using Microsoft.Extensions.Options;

namespace Api.WhatsApp.Services;

public class WhatsAppClient
{
    private readonly HttpClient _http;
    private readonly WhatsAppOptions _options;

    public WhatsAppClient(HttpClient http, IOptions<WhatsAppOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    /// <summary>
    /// Baixa a midia enviada pelo usuario: primeiro descobre a URL temporaria,
    /// depois baixa o binario. Ambas as chamadas exigem o Access Token.
    /// </summary>
    public async Task<(byte[] Bytes, string ContentType)> DownloadMediaAsync(
        string mediaId, CancellationToken ct = default)
    {
        var metaUrl = $"https://graph.facebook.com/{_options.ApiVersion}/{mediaId}";
        using var metaReq = new HttpRequestMessage(HttpMethod.Get, metaUrl);
        metaReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);

        using var metaResp = await _http.SendAsync(metaReq, ct);
        metaResp.EnsureSuccessStatusCode();

        var meta = await metaResp.Content.ReadFromJsonAsync<MediaMeta>(cancellationToken: ct)
                   ?? throw new InvalidOperationException("Resposta de midia invalida.");

        const int maxAttempts = 3;

        for (var attempt = 1; ; attempt++)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                using var fileReq = new HttpRequestMessage(HttpMethod.Get, meta.Url);
                fileReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);

                using var fileResp = await _http.SendAsync(fileReq, ct);
                fileResp.EnsureSuccessStatusCode();

                var contentType = fileResp.Content.Headers.ContentType?.MediaType
                                  ?? meta.MimeType ?? "image/jpeg";
                var bytes = await fileResp.Content.ReadAsByteArrayAsync(ct);

                return (bytes, contentType);
            }
            catch (Exception ex) when (attempt < maxAttempts && IsTransient(ex, ct))
            {
                Console.WriteLine($"[WhatsApp] Download da midia falhou (tentativa {attempt}/{maxAttempts}): {ex.Message}");
                await Task.Delay(TimeSpan.FromMilliseconds(400 * attempt), ct);
            }
        }
    }

    // Transitorio = vale retentar. 4xx (token expirado, midia inexistente) NAO retenta.
    private static bool IsTransient(Exception ex, CancellationToken ct) => ex switch
    {
        HttpRequestException { StatusCode: null } => true,               // rede/TLS (o nosso caso)
        HttpRequestException { StatusCode: { } s } =>
            (int)s >= 500 || (int)s == 408 || (int)s == 429,             // 5xx + timeout + rate limit
        IOException => true,
        TaskCanceledException => !ct.IsCancellationRequested,            // timeout do HttpClient, NAO cancelamento
        _ => false
    };

    private record MediaMeta(
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("mime_type")] string? MimeType);
}
