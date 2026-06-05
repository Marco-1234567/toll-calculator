using System;
using System.Collections.Generic;
using System.Text;
using TollCalculator;
using TollCalculator.Models;

namespace TollCalculator.Tests
{
    public class TollCalculatorTests
    {
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
            var results = new TollFeeCalculator().Calculate(entries);

            // Assert
            Assert.Equal(2, results.Count);
            Assert.Equal(2, results.First(v => v.RegNo == "AAA111").Details.Count);
        }
    }
}
