using Microsoft.EntityFrameworkCore;
using Dominio.Entities;
using Dominio.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class OidConfigurationRepository : IOidConfigurationRepository
    {
        private readonly ApplicationDbContext _context;

        public OidConfigurationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OidConfiguration?> GetByModelIdAsync(int modelId)
        {
            return await _context.Oids
                .Include(o => o.Model)
                .FirstOrDefaultAsync(o => o.ModelId == modelId);
        }

        public async Task<OidConfiguration?> GetByModelNameAsync(string modelName)
        {
            return await _context.Oids
                .Include(o => o.Model)
                .FirstOrDefaultAsync(o => o.Model.Name == modelName);
        }

        public async Task<Dictionary<string, OidConfiguration>> GetAllAsyncDictionary()
        {
            return await _context.Oids
                .Include(o => o.Model)
                .ToDictionaryAsync(o => o.Model.Name, o => o);
        }
    }
}
