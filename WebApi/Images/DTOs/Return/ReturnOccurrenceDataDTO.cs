namespace Api.Images.DTOs.Return;

public record ReturnOccurrenceDataDTO
{
    public Guid Id {get; set;}
    public bool Have_hole {get; set;}
    public double Confidence {get; set;}
    public required string Model_version {get; set;}
    public required string Image64 {get; set;}
    public required string Image64_annotated {get; set;}
}