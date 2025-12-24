using Application.DTOs;
using Application.Interfaces;
using Dominio.Entities;
using Dominio.Enums;
using Dominio.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent; // Necesario para ConcurrentBag

namespace Application.Services
{
    public class PrinterRealtimeService : IPrinterRealtimeService
    {
        private readonly IPrinterRepository _printerRepository;
        private readonly IOidConfigurationRepository _oidConfigRepository;
        private readonly ISnmpService _snmpService;
        private readonly IPrinterHubService _hubService;
        private readonly ILogger<PrinterRealtimeService> _logger;

        public PrinterRealtimeService(
            IPrinterRepository printerRepository,
            IOidConfigurationRepository oidConfigRepository,
            ISnmpService snmpService,
            IPrinterHubService hubService,
            ILogger<PrinterRealtimeService> logger)
        {
            _printerRepository = printerRepository;
            _oidConfigRepository = oidConfigRepository;
            _snmpService = snmpService;
            _hubService = hubService;
            _logger = logger;
        }

        public async Task ScanAllPrintersAsync(string connectionId, int? locationId = null)
        {
            try
            {
                IEnumerable<Printer> printers;

                if (locationId.HasValue && locationId.Value > 0)
                {
                    printers = await _printerRepository.GetByLocationAsync(locationId.Value);
                }
                else
                {
                    printers = await _printerRepository.GetAllAsync();
                }

                if (!printers.Any())
                {
                    await _hubService.SendScanCompletedAsync(connectionId);
                    return;
                }

                var oidConfigs = await _oidConfigRepository.GetAllAsyncDictionary();

                await _hubService.SendScanStartedAsync(connectionId, printers.Count());
                var retryQueue = new ConcurrentBag<Printer>();

                var tasks = printers.Select(async (printer, index) =>
                {
                    try
                    {
                        var dto = await ProcessPrinterAsync(printer, oidConfigs, index + 1, printers.Count());

                        if (dto.Status == "Online" && (string.IsNullOrEmpty(dto.SerialNumber) || dto.SerialNumber == "N/A"))
                        {
                            retryQueue.Add(printer);
                        }

                        await _hubService.SendPrinterDataAsync(connectionId, dto);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error procesando impresora {PrinterId}", printer.Id);
                        await _hubService.SendErrorAsync(connectionId, printer.Id, ex.Message);
                    }
                });
                await Task.WhenAll(tasks);

                await _hubService.SendScanCompletedAsync(connectionId);
                if (!retryQueue.IsEmpty)
                {
                    _ = Task.Run(() => RetryStubbornPrinters(retryQueue.ToList(), oidConfigs, connectionId));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en escaneo general");
                await _hubService.SendErrorAsync(connectionId, 0, ex.Message);
            }
        }

        private async Task RetryStubbornPrinters(List<Printer> printersToRetry, Dictionary<string, OidConfiguration> oidConfigs, string connectionId)
        {
            await Task.Delay(2000);

            foreach (var printer in printersToRetry)
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var dto = await ProcessPrinterAsync(printer, oidConfigs, 0, 0);

                        if (dto.Status == "Online" && dto.SerialNumber != "N/A")
                        {
                            await _hubService.SendPrinterDataAsync(connectionId, dto);

                            break;
                        }

                        await Task.Delay(1000 * (i + 1));
                    }
                    catch
                    {
                    }
                }
            }
        }


        public async Task ScanPrintersByLocationAsync(int locationId, string connectionId)
        {
            await ScanAllPrintersAsync(connectionId, locationId);
        }

        public async Task RefreshSinglePrinterAsync(int printerId, string connectionId)
        {
            try
            {
                var printer = await _printerRepository.GetByIdAsync(printerId);
                if (printer == null) return;
                var oidConfig = await _oidConfigRepository.GetByModelNameAsync(printer.Model.Name);
                if (oidConfig == null)
                {
                    await _hubService.SendErrorAsync(connectionId, printerId, "Modelo no configurado");
                    return;
                }

                var oidConfigs = new Dictionary<string, OidConfiguration>
        {
            { printer.Model.Name, oidConfig }
        };

                var dto = await ProcessPrinterAsync(printer, oidConfigs, 1, 1);

                await _hubService.SendPrinterDataAsync(connectionId, dto);

                if (dto.Status == "Online" && (string.IsNullOrEmpty(dto.SerialNumber) || dto.SerialNumber == "N/A"))
                {
                    var retryList = new List<Printer> { printer };

                    _ = Task.Run(() => RetryStubbornPrinters(retryList, oidConfigs, connectionId));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refrescando impresora {PrinterId}", printerId);
                await _hubService.SendErrorAsync(connectionId, printerId, ex.Message);
            }
        }

        private async Task<PrinterDto> ProcessPrinterAsync(
            Printer printer,
            Dictionary<string, OidConfiguration> oidConfigs,
            int current,
            int total)
        {
            // 1. PING
            var isOnline = await _snmpService.PingPrinterAsync(printer.IpAddress);
            printer.Status = isOnline ? PrinterStatus.Online : PrinterStatus.Offline;

            // 2. SNMP
            if (isOnline && oidConfigs.TryGetValue(printer.Model.Name, out var oidConfig))
            {
                try
                {
                    // Intentamos enriquecer datos
                    await _snmpService.EnrichPrinterDataAsync(printer, oidConfig);
                }
                catch (Exception)
                {
                    SetPrinterAsSnmpFailed(printer);
                }
            }
            else if (!isOnline)
            {
                SetPrinterAsSnmpFailed(printer);
            }

            return new PrinterDto
            {
                Id = printer.Id,
                Model = printer.Model?.Name ?? "Unknown",
                IpAddress = printer.IpAddress,
                Location = printer.LocationText ?? printer.Location?.Name ?? "Unknown",
                MacAddress = printer.MacAddress ?? "N/A",
                SerialNumber = printer.SerialNumber ?? "N/A",
                PageCount = printer.PageCount,
                Status = printer.Status.ToString(),
                TonerLevels = new TonerLevelsDto
                {
                 
                    Black = ValidateLevel(printer.TonerLevels.Black),
                    Cyan = ValidateLevel(printer.TonerLevels.Cyan),
                    Magenta = ValidateLevel(printer.TonerLevels.Magenta),
                    Yellow = ValidateLevel(printer.TonerLevels.Yellow)
                },
                WasteContainerLevel = ValidateLevel(printer.WasteContainerLevel),
                ImageUnitLevel = ValidateLevel(printer.ImageUnitLevel)
            };
        }

        private void SetPrinterAsSnmpFailed(Printer printer)
        {
            printer.MacAddress = "N/A";
            printer.SerialNumber = "N/A";
            printer.PageCount = 0;
            printer.TonerLevels.Black = -1;
            printer.TonerLevels.Cyan = -1;
            printer.TonerLevels.Magenta = -1;
            printer.TonerLevels.Yellow = -1;
            printer.WasteContainerLevel = -1;
            printer.ImageUnitLevel = -1;
        }

        private int? ValidateLevel(int? level)
        {
            return level <= 0 ? null : level;
        }
    }
}