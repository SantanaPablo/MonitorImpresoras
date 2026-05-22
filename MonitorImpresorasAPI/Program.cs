using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Dominio.Interfaces;
using Infrastructure.Repositories;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Services;
using Infrastructure.ExternalServices;
using Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicy = "AllowAll";

// BASE DE DATOS

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// REPOSITORIOS

builder.Services.AddScoped<IPrinterRepository, PrinterRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IOidConfigurationRepository, OidConfigurationRepository>();
builder.Services.AddScoped<IPrinterModelRepository, PrinterModelRepository>();

// SERVICIOS
builder.Services.AddScoped<IPrinterService, PrinterService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IPrinterRealtimeService, PrinterRealtimeService>();
builder.Services.AddScoped<ISnmpService, SnmpService>();
builder.Services.AddScoped<IPrinterHubService, PrinterHubService>();

// SIGNALR, CONTROLLERS Y SWAGGER
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {

        var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }

        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

//
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthorization();

app.MapControllers();
app.MapHub<PrinterHub>("/printerHub");

app.Run();