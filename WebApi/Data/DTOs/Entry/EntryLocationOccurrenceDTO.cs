namespace Api.Data.DTOs.Entry;

public record EntryLocationOccurrenceDTO
{
    public string Phone { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Url { get; set; }
}
