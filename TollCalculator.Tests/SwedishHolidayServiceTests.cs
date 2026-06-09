using TollCalculator.Services;

namespace TollCalculator.Tests
{
    public class SwedishHolidayServiceTests
    {
        private readonly SwedishHolidayService _holidayService;

        public SwedishHolidayServiceTests()
        {
            _holidayService = new SwedishHolidayService();
        }

        [Fact]
        public void IsWeekend_Saturday_ReturnsTrue()
        {
            Assert.True(_holidayService.IsWeekend(new DateTime(2026, 6, 13)));
        }

        [Fact]
        public void IsWeekend_Sunday_ReturnsTrue()
        {
            Assert.True(_holidayService.IsWeekend(new DateTime(2026, 6, 14)));
        }

        [Fact]
        public void IsWeekend_Monday_ReturnsFalse()
        {
            Assert.False(_holidayService.IsWeekend(new DateTime(2026, 6, 15)));
        }

        [Theory]
        [InlineData(1, 1)]   // Nyårsdagen
        [InlineData(1, 6)]   // Trettondedag jul
        [InlineData(5, 1)]   // Första maj
        [InlineData(6, 6)]   // Nationaldagen
        [InlineData(12, 24)] // Julafton
        [InlineData(12, 25)] // Juldagen
        [InlineData(12, 26)] // Annandag jul
        [InlineData(12, 31)] // Nyårsafton
        public void IsPublicHoliday_FixedHoliday_ReturnsTrue(int month, int day)
        {
            // Arrange
            var date = new DateTime(2026, month, day);

            // Act
            var result = _holidayService.IsPublicHoliday(date);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(2026, 6, 19)] // Already a Friday
        [InlineData(2027, 6, 25)] // First Friday after June 19
        public void GetMidsummerEve_ReturnsCorrectDate(int year, int month, int day)
        {
            // Arrange
            var expected = new DateTime(year, month, day);

            // Act
            var result = _holidayService.GetMidsummerEve(year);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
