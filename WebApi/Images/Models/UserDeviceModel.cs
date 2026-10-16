namespace Api.Images.Models;

public class UserDeviceModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string refresh_token {get; set;}
    public DateTime created_at {get; set;} = DateTime.UtcNow;
    public DateTime expires_at {get; set;} = DateTime.UtcNow;
    public bool revoked {get; set;} = false;
    public string device_info {get; set;}
    public string ip_address {get; set;}
    public Guid device_id {get; set;}
    public Guid id_user {get; set;}
}