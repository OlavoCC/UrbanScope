namespace Api.Data.DTOs.Return;

using Api.Data.DTOs.Intra;
public record ComplaintsReturnByNeighborhoodDTO
{
    public List<ComplaintsReturnDTO> ComplaintsByNeighborhood { get; set; }
}