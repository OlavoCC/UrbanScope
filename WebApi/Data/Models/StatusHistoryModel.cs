namespace Api.Data.Models;

public class StatusHistoryModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime changed_at {get; set;} = DateTime.UtcNow;
    public Guid id_user {get; set;}
    public Guid id_complaint {get; set;}
    public Guid id_complaint_status {get; set;}
    
}