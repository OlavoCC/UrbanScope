namespace Api.Images.DTOs.FastApiResponse;

public record YoloDetectResponseDTO
{
    public bool have_hole {get; set;}
    public required YoloConfidenceDTO confidence_max {get; set;}
    public required string model_version {get; set;}
    public required string image_annotated {get; set;}
}

public record YoloConfidenceDTO
{
    public double raw {get; set;}
    public double normalized {get; set;}
}
