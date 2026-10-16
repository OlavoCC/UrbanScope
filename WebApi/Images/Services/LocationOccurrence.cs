namespace Api.Images.Services;

using Api.Images.DTOs.Entry;

public class LocationOccurrence
{
    // TODO: trocar por banco/repositorio
    private static readonly List<EntryLocationOccurrenceDTO> _store = new();

    public Task<Guid> CreateLocation(EntryLocationOccurrenceDTO dto)
    {
        _store.Add(dto);
        Console.WriteLine($"[WhatsApp] Localizacao de {dto.Phone}: {dto.Latitude},{dto.Longitude}");
        return Task.FromResult(Guid.NewGuid());
    }
}
