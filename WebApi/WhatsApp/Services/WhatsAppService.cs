using System.Text.Json;
using Api.Data.DTOs.Intra;
using Api.Images.Services;
using Api.Users.Data.Interfaces;
using Api.WhatsApp.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Api.Occurrences.Interfaces;

namespace Api.WhatsApp.Services;

public class WhatsAppService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<WhatsAppService> _logger;
    private readonly WhatsAppClient _whats;
    private readonly ImageOccurrenceService _images;
    private readonly IDbUser _userDb;
    private readonly IOccurrence _occurrenceService;

    public WhatsAppService(
        IMemoryCache cache,
        ILogger<WhatsAppService> logger,
        WhatsAppClient whats,
        ImageOccurrenceService images,
        IOccurrence occurrenceService,
        IDbUser userDb)
    {
        _cache = cache;
        _logger = logger;
        _whats = whats;
        _images = images;
        _userDb = userDb;
        _occurrenceService = occurrenceService;
    }

    public async Task<bool> ReceiveImageAsync(WebhookTransporterDTO<string> dto)
    {
        var envelope = JsonSerializer.Deserialize<WebhookEnvelope>(dto.RawBody);
        if (envelope?.Entry is null)
            return true;

        foreach (var entry in envelope.Entry)
        {
            foreach (var change in entry.Changes ?? new List<WebhookChange>())
            {
                foreach (var message in change.Value?.Messages ?? new List<WebhookMessage>())
                {
                    // Dedupe: a Meta reentrega a mesma mensagem se o handler falhou ou timeout
                    if (_cache.TryGetValue(message.Id, out _))
                    {
                        _logger.LogInformation("Mensagem {MessageId} ja processada; ignorando reentrega.", message.Id);
                        continue;
                    }
                    _cache.Set(message.Id, true, TimeSpan.FromHours(1));

                    try
                    {
                        switch (message.Type)
                        {
                            case "image" when message.Image is not null:
                                var media = await _whats.DownloadMediaAsync(message.Image.Id);
                                var bytes = media.Bytes;
                                var contentType = media.ContentType;
                                var occurrence = await _images.CreateOccurrence(bytes, contentType, $"{message.Image.Id}.jpg");
                                _logger.LogInformation("[WhatsApp] Foto de {From} -> buraco: {HaveHole} ({Confidence:0.0}%)",
                                    message.From, occurrence.Have_hole, occurrence.Confidence);

                                var phone = message.From;
                                if(occurrence.Confidence > 40)
                                {
                                    var result = await _occurrenceService.CreateOccurrenceIntermedium(phone, bytes, $"{message.Image.Id}.jpg");
                                if (result)
                                {
                                    _logger.LogInformation("[WhatsApp] Ocorrencia criada com sucesso para {From}.", message.From);
                                }
                                }

                                
                                break;

                            case "location" when message.Location is not null:
                                _logger.LogInformation("[WhatsApp] Localizacao de {From}: {Latitude}, {Longitude}",
                                    message.From, message.Location.Latitude, message.Location.Longitude);
                                var CreateNeighborhood = await _occurrenceService.CreateNeighborhood(message.Location.Latitude, message.Location.Longitude);
                                if (!CreateNeighborhood)
                                {
                                    _logger.LogInformation("[API] Erro ao criar o barrio");
                                }
                                var CreateComplaint = await _occurrenceService.CreateComplaint(message.Location.Latitude, message.Location.Longitude, message.From);
                                if (!CreateComplaint)
                                {
                                   _logger.LogInformation("[API] Erro ao criar ocorrencia"); 
                                }
                                
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Falha ao processar mensagem {MessageId} de {From} (tipo {Type}).",
                            message.Id, message.From, message.Type);
                        
                    }
                }
            }
        }

        return true ;
    }
}