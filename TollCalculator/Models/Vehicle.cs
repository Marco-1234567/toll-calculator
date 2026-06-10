namespace TollCalculator.Models
{
    /// <summary>
    /// Abstract base class for all vehicle types.
    /// </summary>
    public abstract class Vehicle
    {
        public string RegNo { get; private set; }

        public Vehicle(string regNo) 
        {
            RegNo = regNo;
        }

        public abstract string GetVehicleType();
    }
}