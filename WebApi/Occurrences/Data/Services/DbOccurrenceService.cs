namespace Api.Occurrences.Data.Services;

using Api.Occurrences.Data.Interfaces;
using Api.Data.DTOs.Return;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Api.Data.DbContext;
using Api.Data.DTOs.Intra;
using Api.Data.Models;

public class DbOccurrenceService : IDbOccurrence
{
    private readonly UrbanScopeContext _context;

    public DbOccurrenceService(UrbanScopeContext context)
    {
        _context = context;
    }
    public async Task<ReturrnTotalOccurrence> TotalOccurrencesDb()
    {
        int TotalPending = await _context.ComplaintStatuses.CountAsync(o => o.status == "PENDENTE");
        int TotalInProgress = await _context.ComplaintStatuses.CountAsync(o => o.status == "EM ANDAMENTO");
        int TotalCompleted = await _context.ComplaintStatuses.CountAsync(o => o.status == "CONCLUIDO");

        return new ReturrnTotalOccurrence
        {
            TotalPending = TotalPending,
            TotalProgress = TotalInProgress,
            TotalComplet = TotalCompleted
        };


    }
    
    public async Task<ComplaintByNeighborhoodWithTotalListDTO> GetComplaintsByNeighborhoodWithTotalDb()
    {
        var statuses = new[] { "PENDENTE", "EM ANDAMENTO" };

        var result = await _context.Complaints
            .Join(_context.ComplaintStatuses,
                c => c.id_complaint_status,
                s => s.Id,
                (c, s) => new { c, s })
            .Join(_context.Neighborhoods,
                x => x.c.id_neighborhood,
                n => n.Id,
                (x, n) => new { x.c, x.s, n })
            .Where(x => statuses.Contains(x.s.status))
            .GroupBy(x => x.n.neighborhood)
            .Select(g => new ComplaintByNeighborhoodDTO
            {
                Neighborhood = g.Key,
                Total = g.Count()
            })
            .ToListAsync();

        return new ComplaintByNeighborhoodWithTotalListDTO
        {
            ComplaintByNeighborhoodList = result
        };
    }

    public async Task<ComplaintByNeighborhoodWithTotalListDTO> ComplaintsOrderedByNeighborhoodDb()
    {
        var statuses = new[] { "PENDENTE", "EM ANDAMENTO" };

        var result = await _context.Complaints
            .Join(_context.ComplaintStatuses,
                c => c.id_complaint_status,
                s => s.Id,
                (c, s) => new { c, s })
            .Join(_context.Neighborhoods,
                x => x.c.id_neighborhood,
                n => n.Id,
                (x, n) => new { x.c, x.s, n })
            .Where(x => statuses.Contains(x.s.status))
            .GroupBy(x => x.n.neighborhood)
            .Select(g => new ComplaintByNeighborhoodDTO
            {
                Neighborhood = g.Key,
                Total = g.Count()
            })
            .OrderByDescending(x => x.Total)   // maior quantidade primeiro
            .ThenBy(x => x.Neighborhood)       // desempate A → Z
            .ToListAsync();

        return new ComplaintByNeighborhoodWithTotalListDTO
        {
            ComplaintByNeighborhoodList = result
        };
    }

    public async Task<ComplaintsReturnDTO> GetComplaintsByIdDb(Guid id)
    {
        
            var complaint = await _context.Complaints
        .Where(c => c.Id == id)
        .Join(_context.ComplaintStatuses,
            c => c.id_complaint_status,
            s => s.Id,
            (c, s) => new { c, s })
        .Join(_context.Neighborhoods,
            x => x.c.id_neighborhood,
            n => n.Id,
            (x, n) => new { x.c, x.s, n })
        .Join(_context.Users,
            x => x.c.id_user,
            u => u.Id,
            (x, u) => new { x.c, x.s, x.n, u })
        .GroupJoin(_context.ComplaintImages,
            x => x.c.Id,
            img => img.id_complaint,
            (x, imgs) => new { x.c, x.s, x.n, x.u, Images = imgs })
        .Select(x => new ComplaintsReturnDTO
        {
            id = x.c.Id,
            phone = x.u.phone,  // Now comes from Users table
            problem = x.c.problem,
            status = x.s.status,
            street = x.c.street,
            neighborhood = x.n.neighborhood,
            register_date = x.c.register_date,
            description = x.c.description,
            image_path = x.Images.FirstOrDefault() != null 
                ? x.Images.First().image_path 
                : string.Empty
        })
        .FirstOrDefaultAsync();

    return complaint;
    }
    public async Task<ComplaintsReturnByNeighborhoodDTO> GetAllComplaintsByNeighborhoodDb(string neighborhood)
    {
            var complaints = await _context.Complaints
    .Join(_context.ComplaintStatuses,
        c => c.id_complaint_status,
        s => s.Id,
        (c, s) => new { c, s })
    .Join(_context.Neighborhoods,
        x => x.c.id_neighborhood,
        n => n.Id,
        (x, n) => new { x.c, x.s, n })
    .Where(x => x.n.neighborhood == neighborhood)
    .GroupJoin(_context.ComplaintImages,
        x => x.c.Id,
        img => img.id_complaint,
        (x, imgs) => new { x.c, x.s, x.n, Images = imgs })
    .Select(x => new NeighborhoodReturnDTO
    {
        id = x.n.Id,
        problem = x.c.problem,
        priority = x.n.priority.ToString(),
        status = x.s.status,
        street = x.c.street,
        neighborhood = x.n.neighborhood,
        register_date = x.c.register_date,
        description = x.c.description ?? string.Empty,
        image_path = x.Images.Any() ? x.Images.First().image_path : string.Empty
    })
    .ToListAsync();

        return new ComplaintsReturnByNeighborhoodDTO
        {
            ComplaintsByNeighborhood = complaints
        };
    }

    public async Task<ComplaintsReturnByNeighborhoodDTO> GetAllComplaintsWithOldestTimeRangeDb(DateTime CurrentDay)
{
    var statuses = new[] { "PENDENTE", "EM ANDAMENTO" };
    var cutoffDate = DateTime.UtcNow.AddDays(-7);

    var complaints = await _context.Complaints
        .Join(_context.ComplaintStatuses,
            c => c.id_complaint_status,
            s => s.Id,
            (c, s) => new { c, s })
        .Join(_context.Neighborhoods,
            x => x.c.id_neighborhood,
            n => n.Id,
            (x, n) => new { x.c, x.s, n })
        .Where(x => statuses.Contains(x.s.status) && x.c.register_date < cutoffDate)
        .OrderBy(x => x.c.register_date)
        .GroupJoin(_context.ComplaintImages,
            x => x.c.Id,
            img => img.id_complaint,
            (x, imgs) => new { x.c, x.s, x.n, Images = imgs })
        .Select(x => new NeighborhoodReturnDTO
        {
            id = x.n.Id,
            problem = x.c.problem,
            priority = x.n.priority.ToString(),
            status = x.s.status,
            street = x.c.street,
            neighborhood = x.n.neighborhood,
            register_date = x.c.register_date,
            description = x.c.description ?? string.Empty,
            image_path = x.Images.Any() ? x.Images.First().image_path : string.Empty
        })
        .ToListAsync();

    return new ComplaintsReturnByNeighborhoodDTO
    {
        ComplaintsByNeighborhood = complaints
    };
}

    public async Task<bool> CreateOccurrenceIntermediumDb(string phone, string url)
    {
        var id_user = await _context.Users
            .Where(u => u.phone == phone)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync();
        if(id_user == null)
        {
            return false;
        }

        var ultimaComplaint = await _context.Complaints
            .Where(c => c.id_user == id_user)
            .OrderByDescending(c => c.Id)
            .Select(c => (Guid?)c.Id) // ou c.Id se não houver data
            .FirstOrDefaultAsync();
        if(ultimaComplaint == null)
        {
            return false;
        }

        var complaintImage = new ComplaintImageModel
        {
            image_path = url,
            id_complaint = ultimaComplaint!.Value
        };
        var addImageComplaint = _context.ComplaintImages.Add(complaintImage);

        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> CreateNeighborhoodDb(string neighborhood)
    {
        var neighborhoodModel = new NeighborhoodModel
        {
            neighborhood = neighborhood,
            priority = NeighborhoodPriority.BAIXA
        };
        var verify = await _context.Neighborhoods
            .Where(n => n.neighborhood == neighborhood)
            .Select(n => (Guid?)n.Id)
            .FirstOrDefaultAsync();
        if(verify == null)
        {
            var insert = _context.Neighborhoods.Add(neighborhoodModel);
            return await _context.SaveChangesAsync() > 0;
        }
        return true;
        

        
    }

    public async Task<bool> CreateComplaintDb(string phone, string neighborhood, string street, double lat, double lon)
    {
        var id_neighborhood = await _context.Neighborhoods
            .Where( n => n.neighborhood == neighborhood)
            .Select(n => (Guid?)n.Id)
            .FirstOrDefaultAsync();
        if (id_neighborhood == null)
        {
            return false;
        }

        var id_user = await _context.Users
            .Where(u => u.phone == phone)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync();
        if(id_user == null)
        {
            return false;
        }

        var id_complaint_status = await _context.ComplaintStatuses
            .Where(c => c.status == "PENDENTE")
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync();
        if(id_complaint_status == null)
        {
            return false;
        }

        var complaintModel = new ComplaintModel
        {
          latitude = lat,
          longitude = lon,
          street = street,
          id_neighborhood = id_neighborhood!.Value,
          id_user = id_user!.Value,
          id_complaint_status = id_complaint_status!.Value
        };

        var complaintAdd = _context.Complaints.Add(complaintModel);
        return await _context.SaveChangesAsync() > 0;
    }

}