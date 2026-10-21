namespace Api.Data.DTOs.Intra;


public record ComplaintsReturnDTO
{
    public Guid id {get; set;}
    public string phone {get; set;}
    public string problem {get; set;}
    public string status {get; set;}
    public string street {get; set;}
    public string neighborhood {get; set;}
    public DateTime register_date {get; set;}
    public string description {get; set;}
    public string image_path {get; set;} 
}