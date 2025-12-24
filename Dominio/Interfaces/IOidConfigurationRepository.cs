using Dominio.Entities;

namespace Dominio.Interfaces
{
    public interface IOidConfigurationRepository
    {
        Task<OidConfiguration?> GetByModelIdAsync(int modelId);
        Task<OidConfiguration?> GetByModelNameAsync(string modelName);
        Task<Dictionary<string, OidConfiguration>> GetAllAsyncDictionary();
    }
}