using Microsoft.EntityFrameworkCore;
using Dominio.Entities;
using Dominio.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class PrinterModelRepository : IPrinterModelRepository
    {
        private readonly ApplicationDbContext _context;

        public PrinterModelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PrinterModel>> GetAllAsync()
        {
            return await _context.Modelos.ToListAsync();
        }

        public async Task<PrinterModel?> GetByIdAsync(int id)
        {
            return await _context.Modelos.FindAsync(id);
        }

        public async Task<PrinterModel?> GetByNameAsync(string name)
        {
            return await _context.Modelos
                .FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task AddAsync(PrinterModel model)
        {
            await _context.Modelos.AddAsync(model);
            await _context.SaveChangesAsync();
        }
    }
}