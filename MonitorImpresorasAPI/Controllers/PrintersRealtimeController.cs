using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces;

namespace MonitorImpresorasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrintersRealtimeController : ControllerBase
    {
        private readonly IServiceScopeFactory _scopeFactory; // Usamos la fábrica
        private readonly ILogger<PrintersRealtimeController> _logger;

        // Inyectamos IServiceScopeFactory en vez del servicio directo
        public PrintersRealtimeController(IServiceScopeFactory scopeFactory, ILogger<PrintersRealtimeController> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        [HttpPost("scan")]
        public IActionResult StartScan([FromBody] ScanRequest request)
        {
            // Lanzamos la tarea en segundo plano
            _ = Task.Run(async () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IPrinterRealtimeService>();

                        // CAMBIO: Pasamos el LocationId (puede ser null o tener valor)
                        await service.ScanAllPrintersAsync(request.ConnectionId, request.LocationId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error en background scan");
                    }
                }
            });

            return Accepted(new { message = "Escaneo iniciado" });
        }

        // (Haz lo mismo con el método RefreshPrinter)
        [HttpPost("refresh/{printerId}")]
        public IActionResult RefreshPrinter(int printerId, [FromBody] ScanRequest request)
        {
            _ = Task.Run(async () =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var realtimeService = scope.ServiceProvider.GetRequiredService<IPrinterRealtimeService>();
                    await realtimeService.RefreshSinglePrinterAsync(printerId, request.ConnectionId);
                }
            });
            return Accepted();
        }
    }

    public class ScanRequest
    {
        public string ConnectionId { get; set; } = string.Empty;
        public int? LocationId { get; set; }
    }
}