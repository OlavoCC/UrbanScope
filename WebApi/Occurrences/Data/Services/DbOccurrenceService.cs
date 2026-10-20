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
}