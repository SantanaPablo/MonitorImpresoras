namespace Dominio.ValueObjects
{
    public class TonerLevels
    {
        public int Black { get; set; }
        public int? Cyan { get; set; }
        public int? Magenta { get; set; }
        public int? Yellow { get; set; }

        public bool HasColorToners => Cyan.HasValue || Magenta.HasValue || Yellow.HasValue;

        public int? GetLowestLevel()
        {
            var levels = new List<int> { Black };
            if (Cyan.HasValue) levels.Add(Cyan.Value);
            if (Magenta.HasValue) levels.Add(Magenta.Value);
            if (Yellow.HasValue) levels.Add(Yellow.Value);

            return levels.Any() ? levels.Min() : null;
        }
    }
}