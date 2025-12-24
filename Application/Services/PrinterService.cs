using Application.DTOs;
using Application.Interfaces;
using Dominio.Entities;
using Dominio.Enums;
using Dominio.Interfaces;

namespace Application.Services
{
    public class PrinterService : IPrinterService
    {
        private readonly IPrinterRepository _printerRepository;
        private readonly IOidConfigurationRepository _oidConfigRepository;
        private readonly ISnmpService _snmpService;

        public PrinterService(
            IPrinterRepository printerRepository,
            IOidConfigurationRepository oidConfigRepository,
            ISnmpService snmpService)
        {
            _printerRepository = printerRepository;
            _oidConfigRepository = oidConfigRepository;
            _snmpService = snmpService;
        }

        public async Task<IEnumerable<PrinterDto>> GetAllPrintersAsync()
        {
            var printers = await _printerRepository.GetAllAsync();
            var oidConfigs = await _oidConfigRepository.GetAllAsyncDictionary();

            var tasks = printers.Select(async printer =>
            {
                await EnrichPrinterWithSnmpData(printer, oidConfigs);
                return MapToDto(printer);
            });

            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<PrinterDto>> GetPrintersByLocationAsync(int locationId)
        {
            var printers = await _printerRepository.GetByLocationAsync(locationId);
            var oidConfigs = await _oidConfigRepository.GetAllAsyncDictionary();

            var tasks = printers.Select(async printer =>
            {
                await EnrichPrinterWithSnmpData(printer, oidConfigs);
                return MapToDto(printer);
            });

            return await Task.WhenAll(tasks);
        }

        public async Task<PrinterDto?> GetPrinterByIdAsync(int id)
        {
            var printer = await _printerRepository.GetByIdAsync(id);
            if (printer == null) return null;

            var oidConfig = await _oidConfigRepository.GetByModelNameAsync(printer.Model.Name);
            if (oidConfig != null)
            {
                await EnrichPrinterWithSnmpData(printer,
                    new Dictionary<string, OidConfiguration> { { printer.Model.Name, oidConfig } });
            }

            return MapToDto(printer);
        }

        private async Task EnrichPrinterWithSnmpData(
            Dominio.Entities.Printer printer,
            Dictionary<string, OidConfiguration> oidConfigs)
        {
            // Verificar conectividad
            var isOnline = await _snmpService.PingPrinterAsync(printer.IpAddress);
            printer.Status = isOnline ? PrinterStatus.Online : PrinterStatus.Offline;

            if (!isOnline)
            {
                printer.MacAddress = "N/A";
                printer.SerialNumber = "N/A";
                return;
            }

            // Obtener configuración de OIDs
            if (!oidConfigs.TryGetValue(printer.Model.Name, out var oidConfig))
                return;

            // Enriquecer con datos SNMP
            await _snmpService.EnrichPrinterDataAsync(printer, oidConfig);
        }

        private PrinterDto MapToDto(Dominio.Entities.Printer printer)
        {
            return new PrinterDto
            {
                Id = printer.Id,
                Model = printer.Model?.Name ?? "Unknown",
                IpAddress = printer.IpAddress,
                Location = printer.LocationText ?? printer.Location?.Name ?? "Unknown",
                LocationId = printer.LocationId ?? 0,
                MacAddress = printer.MacAddress ?? "N/A",
                SerialNumber = printer.SerialNumber ?? "N/A",
                PageCount = printer.PageCount,
                Status = printer.Status.ToString(),
                TonerLevels = new TonerLevelsDto
                {
                    Black = printer.TonerLevels.Black,
                    Cyan = printer.TonerLevels.Cyan,
                    Magenta = printer.TonerLevels.Magenta,
                    Yellow = printer.TonerLevels.Yellow
                },
                WasteContainerLevel = printer.WasteContainerLevel,
                ImageUnitLevel = printer.ImageUnitLevel
            };
        }
    }
}