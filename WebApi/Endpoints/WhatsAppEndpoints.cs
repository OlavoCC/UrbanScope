namespace Api.Endpoints;
using System.Text.Json;
using Api.Images.Services;
using Api.WhatsApp.Config;
using Api.WhatsApp.DTOs;
using Api.WhatsApp.Services;
using Microsoft.Extensions.Options;
using Api.Data.DTOs.Intra;
using Api.Users.Data.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Api.WhatsApp.Services;

public static class WhatsAppEndpoints
{
    public static void MapWhatsAppEndpoints(this IEndpointRouteBuilder app)
    {
        var whats = app.MapGroup("/api/whatsapp")
            .WithTags("WhatsApp");

            whats.MapPost("/Webhook", async (
                HttpRequest req,
                IOptions<WhatsAppOptions> opt,
                WhatsAppService service) =>
            {
                using var ms = new MemoryStream();
                await req.Body.CopyToAsync(ms);
                var rawBody = ms.ToArray();
            
                if (!WhatsAppSignature.IsValid(rawBody, req.Headers["X-Hub-Signature-256"], opt.Value.AppSecret))
                    return Results.StatusCode(StatusCodes.Status401Unauthorized);
            
                var dto = new WebhookTransporterDTO<string> { RawBody = System.Text.Encoding.UTF8.GetString(rawBody) };
                bool success = await service.ReceiveImageAsync(dto);
                if (success)
                {
                    return Results.Ok();
                }
                else{
                    return Results.Problem();
                }
            });

        // Handshake do webhook (a Meta chama isso pra validar a URL)
        whats.MapGet("/Api/WhatsApp/Webhook", (HttpRequest req, IOptions<WhatsAppOptions> opt) =>
        {
            var mode = (string?)req.Query["hub.mode"];
            var token = (string?)req.Query["hub.verify_token"];
            var challenge = (string?)req.Query["hub.challenge"];

            if (mode == "subscribe" && token == opt.Value.VerifyToken)
                return Results.Text(challenge ?? "", "text/plain");

            return Results.StatusCode(StatusCodes.Status403Forbidden);
        });
    }
}