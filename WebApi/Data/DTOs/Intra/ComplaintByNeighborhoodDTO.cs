namespace Api.Data.DTOs.Intra;

public record ComplaintByNeighborhoodDTO
{
    public string Neighborhood {get; set;}
    public int Total {get; set;}
}