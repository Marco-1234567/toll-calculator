namespace TollCalculator.Models
{
    public class Car : Vehicle
    {
        public Car(string regNo): base(regNo) { }

        public override string GetVehicleType()
        {
            return "Car";
        }
    }
}