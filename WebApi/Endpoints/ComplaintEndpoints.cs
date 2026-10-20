namespace Api.Endpoints;

using Api.Occurrences.Interfaces;
public static class ComplaintEndpoints
{
    public static void MapComplaintEndpoints(this IEndpointRouteBuilder app)
    {
        var complaints = app.MapGroup("/api/complaints")
            .WithTags("Complaints");

        complaints.MapGet("/GetTotalComplaints", async (IOccurrence _occurrence) =>
        {
            var result = await _occurrence.TotalOccurrences();
            if (result != null)
            {
                return Results.Ok(result);
            }
            return Results.NotFound();
        });

        complaints.MapGet("/GetComplaintsByNeighborhoodWithTotal", async (IOccurrence _occurrence) =>
        {
            var result = await _occurrence.GetComplaintsByNeighborhoodWithTotal();
            if (result != null)
            {
                return Results.Ok(result);
            }
            return Results.NotFound();
        });

        complaints.MapGet("/ComplaintsOrderedByNeighborhood", async (IOccurrence _occurrence) =>
        {
            var result = await _occurrence.ComplaintsOrderedByNeighborhood();
            if (result != null)
            {
                return Results.Ok(result);
            }
            return Results.NotFound();
        });

        complaints.MapGet("/GetComplaintsById/{id}", async (Guid id, IOccurrence _occurrence) =>
        {
            var result = await _occurrence.GetComplaintsById(id);
            if (result != null)
            {
                return Results.Ok(result);
            }
            return Results.NotFound();
        });
        
        complaints.MapGet("/GetAllComplaintsByNeighborhood/{neighborhood}", async (string neighborhood, IOccurrence _occurrence) =>
        {
            var result = await _occurrence.GetAllComplaintsByNeighborhood(neighborhood);
            if (result != null)
            {
                return Results.Ok(result);
            }
            return Results.NotFound();
        });

        complaints.MapGet("/GetAllComplaintsWithOldestTimeRange", async (IOccurrence _occurrence) =>
        {
            var result = await _occurrence.GetAllComplaintsWithOldsetTimeRange();
            if (result != null)
            {
                return Results.Ok(result);
            }
            return Results.NotFound();
        });
    }
}
