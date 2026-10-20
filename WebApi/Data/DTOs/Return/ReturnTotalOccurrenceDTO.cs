namespace Api.Data.DTOs.Return;

public record ReturrnTotalOccurrence{
    public int TotalPending { get; set; }
    public int TotalProgress { get; set; }
    public int TotalComplet { get; set; }
}