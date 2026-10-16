using System.Text.Json.Serialization;

namespace Api.WhatsApp.DTOs;

public record WebhookEnvelope(
    [property: JsonPropertyName("object")] string? Kind,
    [property: JsonPropertyName("entry")] List<WebhookEntry>? Entry);

public record WebhookEntry(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("changes")] List<WebhookChange>? Changes);

public record WebhookChange(
    [property: JsonPropertyName("field")] string? Field,
    [property: JsonPropertyName("value")] WebhookValue? Value);

public record WebhookValue(
    [property: JsonPropertyName("metadata")] WebhookMetadata? Metadata,
    [property: JsonPropertyName("messages")] List<WebhookMessage>? Messages);

public record WebhookMetadata(
    [property: JsonPropertyName("display_phone_number")] string? DisplayPhoneNumber,
    [property: JsonPropertyName("phone_number_id")] string? PhoneNumberId);

public record WebhookMessage(
    [property: JsonPropertyName("from")] string From,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("timestamp")] string? Timestamp,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("image")] WebhookImage? Image,
    [property: JsonPropertyName("location")] WebhookLocation? Location);

public record WebhookImage(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("mime_type")] string? MimeType,
    [property: JsonPropertyName("sha256")] string? Sha256);

public record WebhookLocation(
    [property: JsonPropertyName("latitude")] double Latitude,
    [property: JsonPropertyName("longitude")] double Longitude,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("address")] string? Address,
    [property: JsonPropertyName("url")] string? Url);
