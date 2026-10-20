namespace Api.Data.Models;

public class ComplaintModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string problem { get; set; } = null!;
    public DateTime register_date { get; set; } = DateTime.UtcNow;
    public decimal latitude { get; set; }
    public decimal longitude { get; set; }
    public string street {get; set;} = null!;
    public string? description {get; set;}
    public Guid id_neighborhood {get; set;}
    public Guid id_user {get; set;}
    public Guid id_complaint_status {get; set;}
}