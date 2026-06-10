namespace TollCalculator.Models
{
    public class Buss : Vehicle
    {
        public Buss(string regNo): base(regNo) { }

        public override string GetVehicleType()
        {
            return "Buss";
        }
    }
}
