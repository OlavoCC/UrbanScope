namespace Api.Data.DTOs.Return;

using Api.Data.DTOs.Intra;


public record ComplaintByNeighborhoodWithTotalListDTO
{
    public List<ComplaintByNeighborhoodDTO> ComplaintByNeighborhoodList {get; set;}
}