namespace Api.Images.Models;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? name { get; set; }
    public string? last_name { get; set; }
    public string phone {get; set;} = null!;
    public string? email { get; set; }
    public string? cpf { get; set; }
    public string? password { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow; 

    //public DateTime? last_login_at {get; set;} = DateTime.UtcNow;
    public DateTime? last_login_at { get; set; }
    public bool? is_active {get; set;} = true;
}
