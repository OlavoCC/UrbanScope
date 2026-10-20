namespace Api.Occurrences.Interfaces;

using Api.Data.DTOs.Return;

public interface IOccurrence
{
    Task<ReturrnTotalOccurrence> TotalOccurrences();
    Task<ComplaintByNeighborhoodWithTotalListDTO> GetComplaintsByNeighborhoodWithTotal();
    Task<ComplaintByNeighborhoodWithTotalListDTO> ComplaintsOrderedByNeighborhood();
}