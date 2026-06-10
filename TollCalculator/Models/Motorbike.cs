namespace TollCalculator.Models
{
    public class Motorbike : Vehicle
    {
        public Motorbike(string regNo) : base(regNo)
        {}

        public override string GetVehicleType()
        {
            return "Motorbike";
        }
    }
}
