namespace TollCalculator.Models
{
    /// <summary>
    /// Represents a standard car subject to toll fees.
    /// </summary>
    public class Car : Vehicle
    {
        public Car(string regNo): base(regNo) { }

        public override string GetVehicleType()
        {
            return "Car";
        }
    }
}