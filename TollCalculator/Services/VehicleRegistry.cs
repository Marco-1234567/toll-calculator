using System;
using System.Collections.Generic;
using System.Text;
using TollCalculator.Models;

namespace TollCalculator.Services
{
    public class VehicleRegistry
    {
        private readonly Dictionary<string, Vehicle> _vehicles;

        public VehicleRegistry(IEnumerable<Vehicle> vehicles)
        {
            _vehicles = vehicles.ToDictionary(v => v.RegNo);
        }

        public Vehicle? GetVehicle(string regNo)
        {
            return _vehicles.TryGetValue(regNo, out Vehicle? vehicle)
                ? vehicle
                : null;
        }
    }
}
