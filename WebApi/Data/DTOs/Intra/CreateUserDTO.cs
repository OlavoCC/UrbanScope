namespace Api.Data.DTOs.Intra;

public record CreateUserDTO{

    public string? name { get; set; }
    public string? last_name { get; set; }
    public string phone {get; set;} = null!;
    public string? email { get; set; }
    public string? cpf { get; set; }
    public string? password { get; set; }
    public DateTime? created_at { get; set; } = DateTime.UtcNow;
}
