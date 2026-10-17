namespace Api.Data.Models;

public class RoleModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string role {get; set;} = null!;
}