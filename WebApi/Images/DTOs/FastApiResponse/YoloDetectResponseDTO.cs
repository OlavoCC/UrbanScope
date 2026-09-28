namespace Api.Images.DTOs.FastApiResponse;

public record YoloDetectResponseDTO
{
    public bool have_hole {get; set;}
    public YoloConfidenceDTO confidence_max {get; set;}
    public string model_version {get; set;}
    public string image_annotated {get; set;}
}

public record YoloConfidenceDTO
{
    public double raw {get; set;}
    public double normalized {get; set;}
}
