namespace Api.Occurrences.Data.Interfaces;

using Api.Data.DTOs.Return;
public interface IDbOccurrence
{
    Task<ReturrnTotalOccurrence> TotalOccurrencesDb();
    Task<ComplaintByNeighborhoodWithTotalListDTO> GetComplaintsByNeighborhoodWithTotalDb();
    Task<ComplaintByNeighborhoodWithTotalListDTO> ComplaintsOrderedByNeighborhoodDb();
}