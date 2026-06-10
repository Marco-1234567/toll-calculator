using TollCalculator.Models;

namespace TollCalculator.Services
{
    public class VehicleRegistry
    {
        private readonly Dictionary<string, Vehicle> _vehicles;

        public VehicleRegistry(IEnumerable<Vehicle> vehicles)
        {
            _vehicles = new Dictionary<string, Vehicle>();
            foreach (var vehicle in vehicles)
            {
                if (_vehicles.ContainsKey(vehicle.RegNo))
                    throw new ArgumentException($"Duplicate RegNo: {vehicle.RegNo}");
                _vehicles[vehicle.RegNo] = vehicle;
            }
        }

        public Vehicle? GetVehicle(string regNo)
        {
            return _vehicles.TryGetValue(regNo, out Vehicle? vehicle)
                ? vehicle
                : null;
        }
    }
}
