namespace TollCalculator.Models
{
    /// <summary>
    /// Represents a vehicle passing through a toll station.
    /// </summary>
    public class TollEntry
    {
        public string RegNo { get; private set; }
        public DateTime EntryTime { get; private set; }

        public TollEntry(string regno, DateTime dt)
        {
            RegNo = regno;
            EntryTime = dt;
        }
    }
}
