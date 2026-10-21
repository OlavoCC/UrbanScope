namespace Api.Data.DTOs.Intra;

public record ComplaintByNeighborhoodDTO
{
    public Guid id {get; set;}
    public string Neighborhood {get; set;}
    public string priority {get; set;}
    public int Total {get; set;}
}