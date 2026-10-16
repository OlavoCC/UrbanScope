namespace Api.Images.DTOs.Return;

public record OccurrenceViewDTO
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = "";
    public string Type { get; set; } = ""; // "image" | "location"
    public DateTimeOffset ReceivedAt { get; set; }
    public bool? HaveHole { get; set; }
    public double? Confidence { get; set; }
    public string? ModelVersion { get; set; }
    public string? Image { get; set; }
    public string? ImageAnnotated { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
