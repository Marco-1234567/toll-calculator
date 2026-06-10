namespace TollCalculator.Models
{
    /// <summary>
    /// Represents a bus exempt from toll fees.
    /// </summary>
    public class Buss : Vehicle
    {
        public Buss(string regNo): base(regNo) { }

        public override string GetVehicleType()
        {
            return "Buss";
        }
    }
}
