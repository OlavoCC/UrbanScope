namespace Api.Data.DTOs.Entry;

public record EditComplaintDTO
{
    public Guid Id { get; set; }
    public string problem { get; set; }
    public string street { get; set; }
    public string description { get; set; }
    public string status { get; set; }
    public string neighborhood { get; set; }
}