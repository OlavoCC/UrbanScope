namespace Api.Data.Models;
public class ComplaintImageModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string image64 { get; set; } = null!;
    public DateTime upload_date { get; set; } = DateTime.UtcNow;
    public Guid id_complaint {get; set;}
    
}