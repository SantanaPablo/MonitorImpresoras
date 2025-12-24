using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class PrinterHubService : IPrinterHubService
    {
        private readonly IHubContext<PrinterHub> _hubContext;

        public PrinterHubService(IHubContext<PrinterHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendScanStartedAsync(string connectionId, int totalPrinters)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("ScanStarted", new
            {
                totalPrinters,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task SendPrinterDataAsync(string connectionId, PrinterDto printer)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("PrinterDataReceived", printer);
        }

        public async Task SendScanCompletedAsync(string connectionId)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("ScanCompleted", new
            {
                timestamp = DateTime.UtcNow
            });
        }

        public async Task SendErrorAsync(string connectionId, int printerId, string error)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("PrinterError", new
            {
                printerId,
                error,
                timestamp = DateTime.UtcNow
            });
        }
    }
}