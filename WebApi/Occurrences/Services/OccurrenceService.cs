namespace Api.Occurrences.Services;

using Api.Occurrences.Interfaces;
using Api.Data.DTOs.Return;
using Api.Occurrences.Data.Interfaces;
using Api.Data.DTOs.Intra;
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

    public async Task<ComplaintsReturnDTO> GetComplaintsById(Guid id)
    {
        return await _dbOccurrence.GetComplaintsByIdDb(id);
    }

    public async Task<ComplaintsReturnByNeighborhoodDTO> GetAllComplaintsByNeighborhood(string neighborhood)
    {
        return await _dbOccurrence.GetAllComplaintsByNeighborhoodDb(neighborhood);
    }
    public async Task<ComplaintsReturnByNeighborhoodDTO> GetAllComplaintsWithOldsetTimeRange()
    {
        DateTime currentDateTime = DateTime.Now;

        return await _dbOccurrence.GetAllComplaintsWithOldestTimeRangeDb(currentDateTime);
    }

}