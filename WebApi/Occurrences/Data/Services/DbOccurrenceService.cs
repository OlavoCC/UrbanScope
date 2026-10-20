namespace Api.Occurrences.Data.Services;

using Api.Occurrences.Data.Interfaces;
using Api.Data.DTOs.Return;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Api.Data.DbContext;
using Api.Data.DTOs.Intra;

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
        .Join(_context.ComplaintStatuses,
            c => c.id_complaint_status,
            s => s.Id,
            (c, s) => new { c, s })
        .Join(_context.Neighborhoods,
            x => x.c.id_neighborhood,
            n => n.Id,
            (x, n) => new { x.c, x.s, n })
        .GroupJoin(_context.ComplaintImages,
            x => x.c.Id,
            img => img.id_complaint,
            (x, imgs) => new { x.c, x.s, x.n, Images = imgs })
        .Select(x => new ComplaintsReturnDTO
        {
            id = x.c.Id,
            problem = x.c.problem,
            status = x.s.status,
            street = x.c.street,
            neighborhood = x.n.neighborhood,
            register_date = x.c.register_date,
            description = x.c.description,
            base64Image = x.Images.FirstOrDefault() != null 
                ? x.Images.First().image64 
                : string.Empty
        })
        .FirstOrDefaultAsync(o => o.id == id);

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
        .Where(x => x.n.neighborhood == neighborhood) // filtra pelo bairro (Guid)
        .GroupJoin(_context.ComplaintImages,
            x => x.c.Id,
            img => img.id_complaint,
            (x, imgs) => new { x.c, x.s, x.n, Images = imgs })
        .Select(x => new ComplaintsReturnDTO
        {
            id = x.c.Id,
            problem = x.c.problem,
            status = x.s.status,
            street = x.c.street,
            neighborhood = x.n.neighborhood,
            register_date = x.c.register_date,
            description = x.c.description ?? string.Empty,
            base64Image = x.Images.Any() ? x.Images.First().image64 : string.Empty
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
            .OrderBy(x => x.c.register_date)   // mais antiga -> menos antiga
            .GroupJoin(_context.ComplaintImages,
                x => x.c.Id,
                img => img.id_complaint,
                (x, imgs) => new { x.c, x.s, x.n, Images = imgs })
            .Select(x => new ComplaintsReturnDTO
            {
                id = x.c.Id,
                problem = x.c.problem,
                status = x.s.status,
                street = x.c.street,
                neighborhood = x.n.neighborhood,
                register_date = x.c.register_date,
                description = x.c.description ?? string.Empty,
                base64Image = x.Images.Any() ? x.Images.First().image64 : string.Empty
            })
            .ToListAsync();

        return new ComplaintsReturnByNeighborhoodDTO
        {
            ComplaintsByNeighborhood = complaints
        };
    }

}