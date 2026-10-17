namespace Api.Data.Models;

public class ComplaintStatusModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string status {get; set;} = null!;
}