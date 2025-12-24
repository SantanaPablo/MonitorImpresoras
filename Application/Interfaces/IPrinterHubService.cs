using Application.DTOs;

namespace Application.Interfaces
{
    public interface IPrinterHubService
    {
        Task SendScanStartedAsync(string connectionId, int totalPrinters);
        Task SendPrinterDataAsync(string connectionId, PrinterDto printer);
        Task SendScanCompletedAsync(string connectionId);
        Task SendErrorAsync(string connectionId, int printerId, string error);
    }
}