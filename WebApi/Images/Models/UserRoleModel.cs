
namespace Api.Images.Models;
public class UserRoleModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid id_user { get; set; }
    public Guid id_role { get; set; }
}