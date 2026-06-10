using TollCalculator.Models;
using TollCalculator.Services;

namespace TollCalculator.Tests
{
    public class VehicleRegistryTests
    {
        [Fact]
        public void VehicleRegistry_DuplicateRegNo_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new VehicleRegistry(new List<Vehicle>
                {
            new Car("CAR123"),
            new Car("CAR123") // duplicate!
                })
            );
        }
    }
}
