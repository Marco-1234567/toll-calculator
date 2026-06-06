using System;
using System.Collections.Generic;
using System.Text;
using TollCalculator;
using TollCalculator.Models;
using TollCalculator.Services;

namespace TollCalculator.Tests
{
    public class TollCalculatorServiceTests
    {
        private readonly TollCalculatorService _calculator;
        private readonly VehicleRegistry _registry;

        public TollCalculatorServiceTests()
        {
            _registry = new VehicleRegistry(new List<Vehicle>
        {
            new Car("BIL123"),
            new Buss("BUS888"),
            new Car("AAA111"),
            new Car("BBB222")
        });
            _calculator = new TollCalculatorService(_registry);
        }

        [Fact]
        public void Calculate_EntriesForTwoVehicles_GroupedByRegNo()
        {
            // Arrange
            var entries = new List<TollEntry>
            {
                new TollEntry( "AAA111", new DateTime(2026, 6, 7, 8, 10, 11)),
                new TollEntry( "BBB222", new DateTime(2026, 6, 7, 8, 15, 15)),
                new TollEntry( "AAA111", new DateTime(2026, 6, 7, 8, 20, 20))
            };
            
            // Act
            var results = _calculator.Calculate(entries);

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Equal(2, results.First(v => v.RegNo == "AAA111").Details.Count);
        }

        [Fact]
        public void Calculate_TollFreeVehicle_ReturnsZeroFee()
        {
            // Arrange
            var entries = new List<TollEntry>
            {
                new TollEntry("BIL123", new DateTime(2026, 6, 8, 8, 0, 0)),
                new TollEntry("BUS888", new DateTime(2026, 6, 8, 8, 0, 1))
            };

            // Act
            var results = _calculator.Calculate(entries);

            // Assert
            var bussFee = results.First(v => v.RegNo == "BUS888").TotalFee;
            var carFee = results.First(v => v.RegNo == "BIL123").TotalFee;

            Assert.Equal(0, bussFee);
            Assert.True(carFee > 0);
        }
    }
}
