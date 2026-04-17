using Xunit;
using RangerFinder.Core.Models;

namespace RangerFinder.Core.Tests.Models;

public class SensorDataTests
{
    [Fact]
    public void Constructor_StoresPassedValues()
    {
        var sensorData = new SensorData(1.1, 2.2, 3.3, 4.4, 5.5);

        Assert.Equal(1.1, sensorData.Ultrasonic1);
        Assert.Equal(2.2, sensorData.Ultrasonic2);
        Assert.Equal(3.3, sensorData.Lidar1);
        Assert.Equal(4.4, sensorData.Lidar2);
        Assert.Equal(5.5, sensorData.Obstacle);
    }
}
