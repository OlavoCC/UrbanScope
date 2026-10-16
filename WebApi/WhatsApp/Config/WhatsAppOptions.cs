namespace Api.WhatsApp.Config;

public class WhatsAppOptions
{
    public const string SectionName = "WhatsApp";

    public string ApiVersion { get; set; } = "v25.0";
    public string VerifyToken { get; set; } = "";
    public string AccessToken { get; set; } = "";
    public string PhoneNumberId { get; set; } = "";
    public string AppSecret { get; set; } = "";
}
