using Api.Images.DTOs.Entry;
using Api.Images.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ImageOccurrence>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger UI em /swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//////////
//ROUTES//
//////////

app.MapPost("/Api/CreateOccurrence", async ([FromForm] EntryOccurrenceDataDTO dto, ImageOccurrence service) =>
{
    var result = await service.CreateOccurrence(dto);
    return Results.Ok(result);
}).DisableAntiforgery(); // API sem cookies -- nao precisa de token CSRF

app.Run();

