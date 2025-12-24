namespace Dominio.Entities
{
    public class OidConfiguration
    {
        public int Id { get; set; }

        // Relación
        public int ModelId { get; set; }
        public PrinterModel Model { get; set; }

        // OIDs básicos
        public string OidMac { get; set; }
        public string OidModel { get; set; }
        public string OidSerial { get; set; }
        public string OidPageCount { get; set; }

        // OIDs de toner negro (obligatorios)
        public string OidBlackToner { get; set; }
        public string OidBlackTonerFull { get; set; }

        // OIDs de toner color (opcionales)
        public string? OidCyanToner { get; set; }
        public string? OidCyanTonerFull { get; set; }
        public string? OidMagentaToner { get; set; }
        public string? OidMagentaTonerFull { get; set; }
        public string? OidYellowToner { get; set; }
        public string? OidYellowTonerFull { get; set; }

        // OIDs adicionales (opcionales)
        public string? OidWasteContainer { get; set; }
        public string? OidUnitImage { get; set; }
        public string? OidUnitImageFull { get; set; }

        // Método helper para verificar si tiene color
        public bool HasColorToner =>
            !string.IsNullOrWhiteSpace(OidCyanToner) &&
            !string.IsNullOrWhiteSpace(OidCyanTonerFull);
    }
}