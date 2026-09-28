namespace Api.Images.DTOs.Return;

public record ReturnOccurrenceDataDTO
{
    public Guid Id {get; set;}
    public bool Have_hole {get; set;}
    public double Confidence {get; set;}
    public string Model_version {get; set;}
    public string Image64 {get; set;}
    public string Image64_annotated {get; set;}
}