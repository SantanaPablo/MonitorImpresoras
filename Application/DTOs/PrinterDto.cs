namespace Application.DTOs
{
    public class PrinterDto
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }

        public int LocationId { get; set; }
        public string MacAddress { get; set; }
        public string SerialNumber { get; set; }
        public int PageCount { get; set; }
        public string Status { get; set; }

        public TonerLevelsDto TonerLevels { get; set; }
        public int? WasteContainerLevel { get; set; }
        public int? ImageUnitLevel { get; set; }
    }

    public class TonerLevelsDto
    {
        public int? Black { get; set; }
        public int? Cyan { get; set; }
        public int? Magenta { get; set; }
        public int? Yellow { get; set; }
    }
}
