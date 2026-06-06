using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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