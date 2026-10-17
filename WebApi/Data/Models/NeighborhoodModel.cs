namespace Api.Data.Models;

public class NeighborhoodModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string neighborhood { get; set; } = null!;
    public NeighborhoodPriority priority { get; set; }
}
