namespace Api.Occurrences.Interfaces;

using Api.Data.DTOs.Return;
using Api.Data.DTOs.Intra;

public interface IOccurrence
{
    Task<ReturrnTotalOccurrence> TotalOccurrences();
    Task<ComplaintByNeighborhoodWithTotalListDTO> GetComplaintsByNeighborhoodWithTotal();
    Task<ComplaintByNeighborhoodWithTotalListDTO> ComplaintsOrderedByNeighborhood();
    Task<ComplaintsReturnDTO> GetComplaintsById(Guid id);
    Task<ComplaintsReturnByNeighborhoodDTO> GetAllComplaintsByNeighborhood(string neighborhood);
    Task<ComplaintsReturnByNeighborhoodDTO> GetAllComplaintsWithOldsetTimeRange();
    Task<bool> CreateOccurrenceIntermedium();
}