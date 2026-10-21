namespace Api.Occurrences.Data.Services;

using Api.Occurrences.Data.Interfaces;
using Api.Data.DTOs.Return;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Api.Data.DbContext;
using Api.Data.DTOs.Intra;
using Api.Data.Models;
using Api.Data.DTOs.Entry;

public class DbOccurrenceService : IDbOccurrence
{
    private readonly UrbanScopeContext _context;

    public DbOccurrenceService(UrbanScopeContext context)
    {
        _context = context;
    }
    public async Task<ReturrnTotalOccurrence> TotalOccurrencesDb()
    {
        var statusCounts = await _context.Complaints
            .Join(_context.ComplaintStatuses,
                c => c.id_complaint_status,
                s => s.Id,
                (c, s) => s.status)
            .GroupBy(status => status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        int TotalPending = statusCounts.FirstOrDefault(x => x.Status == "PENDENTE")?.Count ?? 0;
        int TotalInProgress = statusCounts.FirstOrDefault(x => x.Status == "EM ANDAMENTO")?.Count ?? 0;
        int TotalCompleted = statusCounts.FirstOrDefault(x => x.Status == "CONCLUIDO")?.Count ?? 0;

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
            .GroupBy(x => new { x.n.Id, x.n.neighborhood, x.n.priority })
            .Select(g => new ComplaintByNeighborhoodDTO
            {
                id = g.Key.Id,
                Neighborhood = g.Key.neighborhood,
                priority = g.Key.priority.ToString(),
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
            .GroupBy(x => new { x.n.Id, x.n.neighborhood, x.n.priority })
            .Select(g => new ComplaintByNeighborhoodDTO
            {
                id = g.Key.Id,
                Neighborhood = g.Key.neighborhood,
                priority = g.Key.priority.ToString(),
                Total = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Neighborhood)
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
            .Select(x => new
            {
                x.c,
                x.s,
                x.n,
                User = _context.Users.Where(u => u.Id == x.c.id_user).FirstOrDefault(),
                FirstImage = _context.ComplaintImages
                    .Where(img => img.id_complaint == x.c.Id)
                    .OrderBy(img => img.Id)
                    .Select(img => img.image_path)
                    .FirstOrDefault()
            })
            .Select(x => new ComplaintsReturnDTO
            {
                id = x.c.Id,
                phone = x.User != null ? x.User.phone : string.Empty,
                problem = x.c.problem,
                status = x.s.status,
                street = x.c.street,
                neighborhood = x.n.neighborhood,
                register_date = x.c.register_date,
                description = x.c.description ?? string.Empty,
                image_path = x.FirstImage ?? string.Empty
            })
            .FirstOrDefaultAsync();

        return complaint;
    }
    public async Task<ComplaintsReturnByNeighborhoodDTO> GetAllComplaintsByNeighborhoodDb(string neighborhood)
    {
        var neighborhoodEntity = await _context.Neighborhoods
            .Where(n => n.neighborhood == neighborhood)
            .Select(n => new { n.Id, n.priority })
            .FirstOrDefaultAsync();

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
                id = x.c.Id,
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
            NeighborhoodId = neighborhoodEntity?.Id ?? Guid.Empty,
            Priority = neighborhoodEntity?.priority.ToString() ?? string.Empty,
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

    public async Task<bool> EditComplaintBd(EditComplaintDTO dto)
    {
        var complaint = await _context.Complaints
            .FirstOrDefaultAsync(c => c.Id == dto.Id);

        if (complaint == null)
            return false;

        if (!string.IsNullOrWhiteSpace(dto.problem))
            complaint.problem = dto.problem;

        if (!string.IsNullOrWhiteSpace(dto.street))
            complaint.street = dto.street;

        if (!string.IsNullOrWhiteSpace(dto.description))
            complaint.description = dto.description;

        if (!string.IsNullOrWhiteSpace(dto.status))
        {
            var statusId = await _context.ComplaintStatuses
                .Where(s => s.status == dto.status)
                .Select(s => (Guid?)s.Id)
                .FirstOrDefaultAsync();
            if (statusId != null)
                complaint.id_complaint_status = statusId.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.neighborhood))
        {
            var neighborhoodId = await _context.Neighborhoods
                .Where(n => n.neighborhood == dto.neighborhood)
                .Select(n => (Guid?)n.Id)
                .FirstOrDefaultAsync();

            if (neighborhoodId == null)
            {
                var newNeighborhood = new NeighborhoodModel
                {
                    neighborhood = dto.neighborhood,
                    priority = NeighborhoodPriority.BAIXA
                };
                _context.Neighborhoods.Add(newNeighborhood);
                await _context.SaveChangesAsync();
                neighborhoodId = newNeighborhood.Id;
            }

            complaint.id_neighborhood = neighborhoodId.Value;
        }

        return await _context.SaveChangesAsync() > 0;
    }

}