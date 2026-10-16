namespace Api.Images.Models;

public class ComplaintModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string problem { get; set; }
    public DateTime register_date { get; set; } = DateTime.UtcNow;
    public float latitude { get; set; }
    public float longitude { get; set; }
    public string street {get; set;}
    public string description {get; set;} = null!;
    public Guid id_neighborhood {get; set;}
    public Guid id_user {get; set;}
    public Guid id_complaint_status {get; set;}
}