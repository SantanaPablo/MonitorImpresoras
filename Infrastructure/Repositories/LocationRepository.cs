using Microsoft.EntityFrameworkCore;
using Dominio.Entities;
using Dominio.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Ubicaciones.OrderBy(l => l.Name).ToListAsync();
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            return await _context.Ubicaciones.FindAsync(id);
        }

        public async Task AddAsync(Location location)
        {
            await _context.Ubicaciones.AddAsync(location);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Location location)
        {
            _context.Ubicaciones.Update(location);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var location = await GetByIdAsync(id);
            if (location != null)
            {
                _context.Ubicaciones.Remove(location);
                await _context.SaveChangesAsync();
            }
        }
    }
}