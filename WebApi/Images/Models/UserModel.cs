namespace Api.Images.Models;

public class UserModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string name { get; set; } = null!;
    public string last_name { get; set; } = null!;
    public string phone {get; set; } = null!;
    public string email { get; set; } = null!;
    public string cpf { get; set; } = null!;
    public string password { get; set; } = null!;
    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public DateTime last_login_at {get; set;} = DateTime.UtcNow;
    public bool is_active {get; set;} = true;
}