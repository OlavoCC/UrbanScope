namespace Api.Occurrences.Services;

using Api.Occurrences.Interfaces;
using Api.Data.DTOs.Return;
using Api.Occurrences.Data.Interfaces;
using Api.Data.DTOs.Intra;
using Api.Users.Data.Interfaces;

using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http.Features;

public class OccurrenceService : IOccurrence
{
    private readonly IDbOccurrence _dbOccurrence;
    private readonly IDbUser _userDb;
    public OccurrenceService(IDbOccurrence dbOccurrence, IDbUser userDb)
    {
        _dbOccurrence = dbOccurrence;
        _userDb = userDb;
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

    public async Task<bool> CreateOccurrenceIntermedium(string phone, byte[] imageBytes, string fileName)
    {
        var create = await _userDb.VerifyUserExistenceAsyncDb(phone);
        if (!create){
            var createUserDTO = new CreateUserDTO { phone = phone };
            var result = await _userDb.CreateUserAsyncDb(createUserDTO);
            if (result)
            {
                
            }
            return true;
        }
        return true;
    }
}