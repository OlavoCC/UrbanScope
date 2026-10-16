namespace Api.Images.Endpoints;
using System.Text.Json;
using Api.Images.Services;
using Api.WhatsApp.Config;
using Api.WhatsApp.DTOs;
using Api.WhatsApp.Services;
using Microsoft.Extensions.Options;
using Api.Images.DTOs.Intra;
using Api.Images.Data.Interfaces;
using Microsoft.Extensions.Caching.Memory;
public static class WhatsAppEndpoints
{
    public static void MapWhatsAppEndpoints(this IEndpointRouteBuilder app)
    {
        var whats = app.MapGroup("/api/whatsapp")
            .WithTags("WhatsApp");

        whats.MapPost("/Webhook", async (HttpRequest req, IOptions<WhatsAppOptions> opt, WhatsAppClient whats, ImageOccurrence images, IDbUser Iuser,IMemoryCache cache, ILogger<Program> logger) =>
        {
            // Corpo cru -- necessario pra validar a assinatura da Meta
            using var ms = new MemoryStream();
            await req.Body.CopyToAsync(ms);
            var rawBody = ms.ToArray();

            if (!WhatsAppSignature.IsValid(rawBody, req.Headers["X-Hub-Signature-256"], opt.Value.AppSecret))
                return Results.StatusCode(StatusCodes.Status401Unauthorized);

            var envelope = JsonSerializer.Deserialize<WebhookEnvelope>(rawBody);
            if (envelope?.Entry is null) return Results.Ok();

            foreach (var entry in envelope.Entry)
            foreach (var change in entry.Changes ?? new())
            foreach (var message in change.Value?.Messages ?? new())
            {
                // Dedupe: a Meta reentrega a MESMA mensagem (falha anterior ou timeout
                // do handler). O id da mensagem e unico, entao serve de chave.
                if (cache.TryGetValue(message.Id, out _))
                {
                    logger.LogInformation("Mensagem {MessageId} ja processada; ignorando reentrega.", message.Id);
                    continue;
                }
                cache.Set(message.Id, true, TimeSpan.FromHours(1));

                try
                {
                    switch (message.Type)
                    {
                        case "image" when message.Image is not null:
                            var (bytes, contentType) = await whats.DownloadMediaAsync(message.Image.Id);
                            var occurrence = await images.CreateOccurrence(bytes, contentType, $"{message.Image.Id}.jpg");
                            logger.LogInformation("[WhatsApp] Foto de {From} -> buraco: {HaveHole} ({Confidence:0.0}%)",
                                message.From, occurrence.Have_hole, occurrence.Confidence);
                            string phone = (message.From).ToString();
                            bool create = await Iuser.VerifyUserExistenceAsync(phone);
                            if(!create){
                                logger.LogInformation("Usuario ainda nao cadastrado");
                                var createUserDTO = new CreateUserDTO
                                {
                                    phone = phone
                                };
                                bool result = await Iuser.CreateUserAsync(createUserDTO);
                                if (result)
                                        {
                                            logger.LogInformation("Usuario criado com sucesso");
                                        }
                                        else
                                        {
                                            logger.LogInformation("Erro ao criar usuario");
                                        }
                            }
                                    else
                                    {
                                        logger.LogInformation("Usuario ja cadastrado");
                                    }
                            break;

                        case "location" when message.Location is not null:
                            logger.LogInformation("[WhatsApp] Localizacao de {From}: {Latitude}, {Longitude}",
                                message.From, message.Location.Latitude, message.Location.Longitude);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Falha ao processar mensagem {MessageId} de {From} (tipo {Type}).",
                        message.Id, message.From, message.Type);
                }
            }

            // Responda 200 rapido (senao a Meta reenvia)
            return Results.Ok();
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