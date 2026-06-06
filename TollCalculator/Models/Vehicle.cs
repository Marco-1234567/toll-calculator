using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TollCalculator.Models
{
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