namespace Api.Data.Models;

public class ComplaintModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string problem { get; set; } = "Buraco";
    public DateTime register_date { get; set; } = DateTime.UtcNow;
    public double latitude { get; set; }
    public double longitude { get; set; }
    public string street {get; set;} = null!;
    public string? description {get; set;}
    public Guid id_neighborhood {get; set;}
    public Guid id_user {get; set;}
    public Guid id_complaint_status {get; set;}
}