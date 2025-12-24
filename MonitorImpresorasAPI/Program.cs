using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Dominio.Interfaces;
using Infrastructure.Repositories;
using Application.Interfaces;
using Application.Services;
using Infrastructure.ExternalServices;
using Infrastructure.Hubs;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Repositorios
builder.Services.AddScoped<IPrinterRepository, PrinterRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IOidConfigurationRepository, OidConfigurationRepository>();
builder.Services.AddScoped<IPrinterModelRepository, PrinterModelRepository>();

// Servicios de Aplicación
builder.Services.AddScoped<IPrinterService, PrinterService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IPrinterRealtimeService, PrinterRealtimeService>();

// Servicios Externos
builder.Services.AddScoped<ISnmpService, SnmpService>();
builder.Services.AddScoped<IPrinterHubService, PrinterHubService>();

// SignalR - NUEVO
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); 
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();
app.MapHub<PrinterHub>("/printerHub");

app.Run();