using Api.Images.Services;
using Api.Endpoints;
using Api.WhatsApp.Config;
using Api.WhatsApp.Services;
using Microsoft.EntityFrameworkCore;
using Api.Data.DbContext;
using Api.Data.Models;
using Npgsql.NameTranslation;
using Api.Users.Data.Interfaces;
using Api.Users.Data.Services;
using Api.Users.Services;
using Api.Users.Interfaces;
using Api.Occurrences.Interfaces;
using Api.Occurrences.Services;
using Api.Occurrences.Data.Interfaces;
using Api.Occurrences.Data.Services;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Api.Data.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy to allow Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));
var settings = builder.Configuration
    .GetSection("Cloudinary")
    .Get<CloudinarySettings>();
var account = new Account(
    settings!.CloudName,
    settings.ApiKey,
    settings.ApiSecret
);
var cloudinary = new Cloudinary(account);
builder.Services.AddSingleton(cloudinary);
builder.Services.AddScoped<CloudinaryService>();


var app = builder.Build();


Console.WriteLine("Conectado ao PostgreSQL!");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Swagger UI em /swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAngularApp");

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
