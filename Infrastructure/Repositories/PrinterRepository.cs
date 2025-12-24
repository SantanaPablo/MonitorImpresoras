using Microsoft.EntityFrameworkCore;
using Dominio.Entities;
using Dominio.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class PrinterRepository : IPrinterRepository
    {
        private readonly ApplicationDbContext _context;

        public PrinterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Printer>> GetAllAsync()
        {
            return await _context.Impresoras
                .Include(p => p.Model)
                .Include(p => p.Location)
                .ToListAsync();
        }

        public async Task<IEnumerable<Printer>> GetByLocationAsync(int locationId)
        {
            return await _context.Impresoras
                .Include(p => p.Model)
                .Include(p => p.Location)
                .Where(p => p.LocationId == locationId)
                .ToListAsync();
        }

        public async Task<Printer?> GetByIdAsync(int id)
        {
            return await _context.Impresoras
                .Include(p => p.Model)
                .Include(p => p.Location)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Printer?> GetByIpAddressAsync(string ipAddress)
        {
            return await _context.Impresoras
                .Include(p => p.Model)
                .Include(p => p.Location)
                .FirstOrDefaultAsync(p => p.IpAddress == ipAddress);
        }

        public async Task AddAsync(Printer printer)
        {
            await _context.Impresoras.AddAsync(printer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Printer printer)
        {
            _context.Impresoras.Update(printer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var printer = await GetByIdAsync(id);
            if (printer != null)
            {
                _context.Impresoras.Remove(printer);
                await _context.SaveChangesAsync();
            }
        }
    }
}