namespace Api.Data.Models;

public class UserDeviceModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string refresh_token {get; set;} = null!;
    public DateTime created_at {get; set;} = DateTime.UtcNow;
    public DateTime expires_at {get; set;} = DateTime.UtcNow;
    public bool revoked {get; set;} = false;
    public string device_info {get; set;} = null!;
    public string ip_address {get; set;} = null!;
    public Guid device_id {get; set;}
    public Guid id_user {get; set;}
}