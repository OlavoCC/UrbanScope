namespace Api.Occurrences.Services;

using Api.Occurrences.Interfaces;
using Api.Data.DTOs.Return;
using Api.Occurrences.Data.Interfaces;
using Api.Data.DTOs.Intra;
using Api.Users.Data.Interfaces;
using Api.Images.Services;
using System.Text.Json;


public class OccurrenceService : IOccurrence
{
    private readonly CloudinaryService _cloudinary;
    private readonly IDbOccurrence _dbOccurrence;
    private readonly IDbUser _userDb;
    private readonly ILogger<OccurrenceService> _logger;
    public OccurrenceService(IDbOccurrence dbOccurrence, IDbUser userDb, CloudinaryService cloudinary, ILogger<OccurrenceService> logger)
    {
        _dbOccurrence = dbOccurrence;
        _userDb = userDb;
        _cloudinary = cloudinary;
        _logger = logger;
    }

    public async Task<JsonElement?> GetAddressAsync(double lat, double lon)
    {
        using var client = new HttpClient();
        var url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}&addressdetails=1";
        client.DefaultRequestHeaders.UserAgent.ParseAdd("UrbanScope/1.0");
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json;
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
                var uploadImage = await _cloudinary.UploadImagemAsync(imageBytes, fileName);
                var complaintImageAdd = await _dbOccurrence.CreateOccurrenceIntermediumDb(phone, uploadImage);
                if (complaintImageAdd)
                {
                    return true;

                }
            }
            return false;
        }
        return true;
    }

    public async Task<bool> CreateNeighborhood(double latitude, double longitude){

        var result = await GetAddressAsync(latitude, longitude);

        if (result is not null)
        {
            var address = result.Value.GetProperty("address");

            var street = address.TryGetProperty("road", out var road)
                ? road.GetString()
                : null;

            var neighborhood = address.TryGetProperty("suburb", out var suburb)
                ? suburb.GetString()
                : null;
            bool createNeighborhood = await _dbOccurrence.CreateNeighborhoodDb(neighborhood);
            if (createNeighborhood)
            {
                return true;
            }
        }

        return false;
    }

    public async Task<bool> CreateComplaint(double lat, double lon, string phone)
    {
        var create = await _userDb.VerifyUserExistenceAsyncDb(phone);

        if (!create)
        {
            var createUserDTO = new CreateUserDTO { phone = phone };
            await _userDb.CreateUserAsyncDb(createUserDTO);
        }
        var result = await GetAddressAsync(lat, lon);

        if (result is not null)
        {
            var address = result.Value.GetProperty("address");

            _logger.LogInformation("=== DEBUG ADDRESS JSON ===");
            foreach (var prop in address.EnumerateObject())
            {
                _logger.LogInformation("  {Name} = {Value}", prop.Name, prop.Value);
            }
            _logger.LogInformation("============================");

            var streetFromAdress = address.TryGetProperty("road", out var road)
                ? road.GetString()
                : null;

            var neighborhoodFromAdress = address.TryGetProperty("suburb", out var suburb)
                ? suburb.GetString()
                : null;

            var createComplaint = await _dbOccurrence.CreateComplaintDb(phone, neighborhoodFromAdress, streetFromAdress, lat, lon);
            if (createComplaint)
            {
                return true;
            }
            return false;
            
        }
        return false;
    }
}