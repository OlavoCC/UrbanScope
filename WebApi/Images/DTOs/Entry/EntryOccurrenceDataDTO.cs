namespace Api.Images.DTOs.Entry;

public record EntryOccurrenceDataDTO
{
    public required IFormFile Image {get; set;}
}