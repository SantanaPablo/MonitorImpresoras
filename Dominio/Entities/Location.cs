namespace Dominio.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Relaciones
        public ICollection<Printer> Printers { get; set; } = new List<Printer>();
    }
}