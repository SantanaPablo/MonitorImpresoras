namespace Dominio.Entities
{
    public class PrinterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Relaciones
        public OidConfiguration? OidConfiguration { get; set; }
        public ICollection<Printer> Printers { get; set; } = new List<Printer>();
    }
}