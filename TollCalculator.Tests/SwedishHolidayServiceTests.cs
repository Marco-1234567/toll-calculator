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
    }
}
