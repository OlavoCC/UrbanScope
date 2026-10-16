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

        using var fileReq = new HttpRequestMessage(HttpMethod.Get, meta.Url);
        fileReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);

        using var fileResp = await _http.SendAsync(fileReq, ct);
        fileResp.EnsureSuccessStatusCode();

        var contentType = fileResp.Content.Headers.ContentType?.MediaType
                          ?? meta.MimeType ?? "image/jpeg";
        var bytes = await fileResp.Content.ReadAsByteArrayAsync(ct);

        return (bytes, contentType);
    }

    private record MediaMeta(
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("mime_type")] string? MimeType);
}
