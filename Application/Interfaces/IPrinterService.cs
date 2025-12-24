using Application.DTOs;

namespace Application.Interfaces
{
    public interface IPrinterService
    {
        Task<IEnumerable<PrinterDto>> GetAllPrintersAsync();
        Task<IEnumerable<PrinterDto>> GetPrintersByLocationAsync(int locationId);
        Task<PrinterDto?> GetPrinterByIdAsync(int id);
    }
}