namespace Api.Data.DTOs.Return;

using Api.Data.DTOs.Intra;
public record ComplaintsReturnByNeighborhoodDTO
{
    public Guid NeighborhoodId { get; set; }
    public string Priority { get; set; }
    public List<NeighborhoodReturnDTO> ComplaintsByNeighborhood { get; set; }
}