using System.Text.Json;
using Api.Images.DTOs.Entry;
using Api.Images.DTOs.Return;
using Api.Images.Services;
using Api.Images.Endpoints;
using Api.WhatsApp.Config;
using Api.WhatsApp.DTOs;
using Api.WhatsApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Api.Images.DTOs.Intra;
using Microsoft.EntityFrameworkCore;
using Api.Images.Context;
using Api.Images.Models;
using Npgsql.NameTranslation;
using Api.Images.Data.Interfaces;
using Images.Data.Services;
using Microsoft.Extensions.Caching.Memory;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ImageOccurrence>();
builder.Services.Configure<WhatsAppOptions>(
builder.Configuration.GetSection(WhatsAppOptions.SectionName));
builder.Services.AddHttpClient<WhatsAppClient>();
builder.Services.AddScoped<IDbUser, DbUserService>();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<UrbanScopeContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MapEnum<NeighborhoodPriority>(
            "priority", null, new NpgsqlNullNameTranslator()))
    .LogTo(Console.WriteLine, LogLevel.Information));

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

app.MapGet("/api/db/ping", async (UrbanScopeContext db) =>
{
    var total = await db.Complaints.CountAsync();
    return Results.Ok(new { conectado = true, complaints = total });
});


app.MapGet("/test/", async (UrbanScopeContext db) =>
{
    var roles = await db.Roles.ToListAsync();
    return Results.Ok(roles);
});

// Recebimento de mensagens (imagem + localizacao)


app.MapUserEndpoints();
app.MapWhatsAppEndpoints();




// Lista das ocorrencias pro front-end
app.Run();
