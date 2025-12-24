using Application.DTOs;

namespace Application.Interfaces
{
    public interface IPrinterRealtimeService
    {
        Task ScanAllPrintersAsync(string connectionId, int? locationId = null);
        Task ScanPrintersByLocationAsync(int locationId, string connectionId);
        Task RefreshSinglePrinterAsync(int printerId, string connectionId);
    }
}
