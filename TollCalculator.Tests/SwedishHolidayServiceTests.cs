using System;
using System.Collections.Generic;
using System.Text;
using TollCalculator.Models;
using TollCalculator.Services;

namespace TollCalculator.Tests
{
    public class SwedishHolidayServiceTests
    {

        private readonly TollCalculatorService _calculator;

        [Fact]
        public void Calculate_EntryOnWeekend_ReturnsZeroFee()
        {
            // Arrange
            var entries = new List<TollEntry>
    {
        new TollEntry("BIL123", new DateTime(2026, 6, 13, 8, 0, 0)), // Saturday
        new TollEntry("BIL123", new DateTime(2026, 6, 14, 8, 0, 0))  // Sunday
    };

            // Act
            var results = _calculator.Calculate(entries);

            // Assert
            Assert.Equal(0, results.First(v => v.RegNo == "BIL123").TotalFee);
        }

    }
}
