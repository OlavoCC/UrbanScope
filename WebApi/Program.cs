using System.Text.Json;
using Api.Images.DTOs.Entry;
using Api.Images.DTOs.Return;
using Api.Images.Services;
using Api.WhatsApp.Config;
using Api.WhatsApp.DTOs;
using Api.WhatsApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ImageOccurrence>();
builder.Services.Configure<WhatsAppOptions>(
    builder.Configuration.GetSection(WhatsAppOptions.SectionName));
builder.Services.AddHttpClient<WhatsAppClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger UI em /swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Em dev, sem redirect: o ngrok precisa postar em HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Front-end estatico (wwwroot/index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

////////////
//ROUTES///
////////////

// ---------------------- WhatsApp ----------------------

// Handshake do webhook (a Meta chama isso pra validar a URL)
app.MapGet("/Api/WhatsApp/Webhook", (HttpRequest req, IOptions<WhatsAppOptions> opt) =>
{
    var mode = (string?)req.Query["hub.mode"];
    var token = (string?)req.Query["hub.verify_token"];
    var challenge = (string?)req.Query["hub.challenge"];

    if (mode == "subscribe" && token == opt.Value.VerifyToken)
        return Results.Text(challenge ?? "", "text/plain");

    return Results.StatusCode(StatusCodes.Status403Forbidden);
});

// Recebimento de mensagens (imagem + localizacao)
app.MapPost("/Api/WhatsApp/Webhook", async (
    HttpRequest req,
    IOptions<WhatsAppOptions> opt,
    WhatsAppClient whats,
    ImageOccurrence images) =>
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
        switch (message.Type)
        {
            case "image" when message.Image is not null:
                var (bytes, contentType) = await whats.DownloadMediaAsync(message.Image.Id);
                var occurrence = await images.CreateOccurrence(bytes, contentType, $"{message.Image.Id}.jpg");
                Console.WriteLine($"[WhatsApp] Foto de {message.From} -> buraco: {occurrence.Have_hole} ({occurrence.Confidence:0.0}%)");
                break;

            case "location" when message.Location is not null:
                Console.WriteLine($"[WhatsApp] Localizacao de {message.From}: {message.Location.Latitude}, {message.Location.Longitude}");
                break;
        }
    }

    // Responda 200 rapido (senao a Meta reenvia)
    return Results.Ok();
});

// Lista das ocorrencias pro front-end
app.Run();
