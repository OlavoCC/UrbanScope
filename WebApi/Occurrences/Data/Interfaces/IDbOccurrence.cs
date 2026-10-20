namespace Api.Occurrences.Data.Interfaces;

using Api.Data.DTOs.Return;
using Api.Data.DTOs.Intra;
public interface IDbOccurrence
{
    Task<ReturrnTotalOccurrence> TotalOccurrencesDb();
    Task<ComplaintByNeighborhoodWithTotalListDTO> GetComplaintsByNeighborhoodWithTotalDb();
    Task<ComplaintByNeighborhoodWithTotalListDTO> ComplaintsOrderedByNeighborhoodDb();
    Task<ComplaintsReturnDTO> GetComplaintsByIdDb(Guid Id);
    Task<ComplaintsReturnByNeighborhoodDTO> GetAllComplaintsByNeighborhoodDb(string neighborhood);
    Task<ComplaintsReturnByNeighborhoodDTO> GetAllComplaintsWithOldestTimeRangeDb(DateTime CurrentDay);
    Task<bool> CreateOccurrenceIntermediumDb(string phone, byte[] imageBytes, string fileName);
}