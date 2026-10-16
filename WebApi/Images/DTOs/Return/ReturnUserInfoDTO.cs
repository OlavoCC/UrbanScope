namespace Api.Images.DTOs.Return;

public record ReturnUserInfoDTO{
    public Guid id {get; set;}
    public string? name {get; set;}
    public string? last_name {get; set;}
    public string phone {get; set;}
    public string? email {get; set;}
    public string? cpf {get; set;}
    public string role {get; set;}
}