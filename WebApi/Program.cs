using System.Text.Json;

using Api.Data.DTOs.Entry;
using Api.Data.DTOs.Return;

using Api.Images.Services;
using Api.Endpoints;
using Api.WhatsApp.Config;
using Api.WhatsApp.DTOs;
using Api.WhatsApp.Services;
using Microsoft.Extensions.Options;
using Api.Data.DTOs.Intra;
using Microsoft.EntityFrameworkCore;
using Api.Data.DbContext;
using Api.Data.Models;
using Npgsql.NameTranslation;
using Api.Users.Data.Interfaces;
using Api.Users.Data.Services;
using Microsoft.Extensions.Caching.Memory;
using Api.Users.Services;
using Api.Users.Interfaces;
using Api.WhatsApp.Services;
using Api.Occurrences.Interfaces;
using Api.Occurrences.Services;
using Api.Occurrences.Data.Interfaces;
using Api.Occurrences.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ImageOccurrenceService>();
builder.Services.AddScoped<IDbUser, DbUserService>();
builder.Services.AddScoped<IOccurrence, OccurrenceService>();
builder.Services.AddScoped<IDbOccurrence, DbOccurrenceService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<WhatsAppService>();
builder.Services.Configure<WhatsAppOptions>(
builder.Configuration.GetSection(WhatsAppOptions.SectionName));
builder.Services.AddHttpClient<WhatsAppClient>();

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<UrbanScopeContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MapEnum<NeighborhoodPriority>(
            "priority", null, new NpgsqlNullNameTranslator()))
    .LogTo(Console.WriteLine, LogLevel.Information));

var app = builder.Build();


Console.WriteLine("Conectado ao PostgreSQL!");

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


////////////
//ROUTES///
////////////


app.MapGet("/api/db/ping", async (UrbanScopeContext db) =>
{
    var total = await db.Complaints.CountAsync();
    return Results.Ok(new { conectado = true, complaints = total });

});


// Recebimento de mensagens (imagem + localizacao)

app.MapUserEndpoints();
app.MapWhatsAppEndpoints();
app.MapComplaintEndpoints();



app.Run();
