using System;
using System.Collections.Generic;
using System.Text;

namespace TollCalculator.Models
{
    public class VehicleFee
    {
        public string RegNo { get; set; }
        public decimal TotalFee { get; set; }
        public required List<VehicleFeeDetails> Details { get; set; }
    }
}
