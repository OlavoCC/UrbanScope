namespace Api.Occurrences.Services;

using Api.Occurrences.Interfaces;
using Api.Data.DTOs.Return;
using Api.Occurrences.Data.Interfaces;

public class OccurrenceService : IOccurrence
{
    private readonly IDbOccurrence _dbOccurrence;
    public OccurrenceService(IDbOccurrence dbOccurrence)
    {
        _dbOccurrence = dbOccurrence;
    }
    public async Task<ReturrnTotalOccurrence> TotalOccurrences()
    {
        return await _dbOccurrence.TotalOccurrencesDb();
    }

    public async Task<ComplaintByNeighborhoodWithTotalListDTO> GetComplaintsByNeighborhoodWithTotal()
    {
        return await _dbOccurrence.GetComplaintsByNeighborhoodWithTotalDb();
    }
    
    public async Task<ComplaintByNeighborhoodWithTotalListDTO> ComplaintsOrderedByNeighborhood()
    {
        return await _dbOccurrence.ComplaintsOrderedByNeighborhoodDb();
    }
}