namespace Api.Images.Models;

public class NeighborhoodModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string neighborhood {get; set;}
    public enum priority
    {
        ALTA,
        MÉDIA,
        BAIXA
    }
}