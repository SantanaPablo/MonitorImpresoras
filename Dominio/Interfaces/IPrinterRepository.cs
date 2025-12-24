using Dominio.Entities;

namespace Dominio.Interfaces
{
    public interface IPrinterRepository
    {
        Task<IEnumerable<Printer>> GetAllAsync();
        Task<IEnumerable<Printer>> GetByLocationAsync(int locationId);
        Task<Printer?> GetByIdAsync(int id);
        Task<Printer?> GetByIpAddressAsync(string ipAddress);
        Task AddAsync(Printer printer);
        Task UpdateAsync(Printer printer);
        Task DeleteAsync(int id);
    }
}