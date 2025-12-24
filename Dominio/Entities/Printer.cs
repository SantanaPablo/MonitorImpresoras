using Dominio.Enums;
using Dominio.ValueObjects;

namespace Dominio.Entities
{
    public class Printer
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public string? LocationText { get; set; }

        // Relaciones
        public int ModelId { get; set; }
        public PrinterModel Model { get; set; }

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        // Datos obtenidos por SNMP (no se almacenan en BD)
        public string? MacAddress { get; set; }
        public string? SerialNumber { get; set; }
        public int PageCount { get; set; }
        public PrinterStatus Status { get; set; }

        // Niveles de toner
        public TonerLevels TonerLevels { get; set; } = new();
        public int WasteContainerLevel { get; set; }
        public int ImageUnitLevel { get; set; }
    }
}