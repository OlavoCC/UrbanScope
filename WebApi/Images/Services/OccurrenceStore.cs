namespace Api.Images.Services;

using Api.Images.DTOs.Return;

/// <summary>
/// Guarda as ocorrencias recebidas (em memoria) para o front-end exibir.
/// Temporario: trocar por banco de dados quando for pra valer.
/// </summary>
public class OccurrenceStore
{
    private const int Max = 100;
    private readonly object _lock = new();
    private readonly List<OccurrenceViewDTO> _items = new();

    public void Add(OccurrenceViewDTO item)
    {
        lock (_lock)
        {
            _items.Insert(0, item); // mais recente primeiro
            if (_items.Count > Max)
                _items.RemoveRange(Max, _items.Count - Max);
        }
    }

    public IReadOnlyList<OccurrenceViewDTO> All()
    {
        lock (_lock)
        {
            return _items.ToList();
        }
    }
}
