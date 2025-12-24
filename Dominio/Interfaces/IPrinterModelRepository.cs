using Dominio.Entities;

namespace Dominio.Interfaces
{
    public interface IPrinterModelRepository
    {
        Task<IEnumerable<PrinterModel>> GetAllAsync();
        Task<PrinterModel?> GetByIdAsync(int id);
        Task<PrinterModel?> GetByNameAsync(string name);
        Task AddAsync(PrinterModel model);
    }
}