using TollCalculator.Models;
using TollCalculator.Services;

namespace TollCalculator.Tests
{
    public class TollCalculatorServiceTests
    {
        private readonly TollCalculatorService _calculator;
        private readonly VehicleRegistry _registry;
        private readonly SwedishHolidayService _holidayService;

        public TollCalculatorServiceTests()
        {
            _registry = new VehicleRegistry(new List<Vehicle>
            {
                new Car("CAR123"),
                new Buss("BUS888"),
                new Car("AAA111"),
                new Car("BBB222")
            });

            _holidayService = new SwedishHolidayService();
            _calculator = new TollCalculatorService(_registry, _holidayService);
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
                new TollEntry("CAR123", new DateTime(2026, 6, 8, 8, 0, 0)),
                new TollEntry("BUS888", new DateTime(2026, 6, 8, 8, 0, 1))
            };

            // Act
            var results = _calculator.Calculate(entries);

            // Assert
            var bussFee = results.First(v => v.RegNo == "BUS888").TotalFee;
            var carFee = results.First(v => v.RegNo == "CAR123").TotalFee;

            Assert.Equal(0, bussFee);
            Assert.True(carFee > 0);
        }

        [Fact]
        public void Calculate_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var entries = new List<TollEntry>();

            // Act
            var results = _calculator.Calculate(entries);

            // Assert
            Assert.Empty(results);
        }

        [Theory]
        [InlineData(6, 0, 8)]
        [InlineData(6, 30, 13)]
        [InlineData(7, 0, 18)]
        [InlineData(8, 30, 8)]
        [InlineData(15, 30, 18)]
        [InlineData(19, 0, 0)]
        public void Calculate_SingleCarEntry_ReturnsCorrectFee(int hour, int minute, int expectedFee)
        {
            // Arrange
            var entries = new List<TollEntry>
    {
        new TollEntry("CAR123", new DateTime(2026, 6, 8, hour, minute, 0)) // Monday
    };

            // Act
            var results = _calculator.Calculate(entries);

            // Assert
            Assert.Equal(expectedFee, results.First().TotalFee);
        }
    }
}
