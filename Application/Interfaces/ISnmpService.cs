using Dominio.Entities;

namespace Application.Interfaces
{
    public interface ISnmpService
    {
        Task<Dictionary<string, string>> GetSnmpValuesAsync(string ipAddress, List<string> oids);
        Task<bool> PingPrinterAsync(string ipAddress);
        Task EnrichPrinterDataAsync(Printer printer, OidConfiguration oidConfig);
    }
}