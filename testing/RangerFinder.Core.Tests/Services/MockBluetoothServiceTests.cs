using Xunit;
using RangerFinder.Core.Models;
using RangerFinder.Core.Services;

namespace RangerFinder.Core.Tests.Services;

public class MockBluetoothServiceTests
{
    [Fact]
    public async Task StartScanningAsync_RaisesDeviceFound()
    {
        var service = new MockBluetoothService();
        var deviceFound = new TaskCompletionSource<(string Name, Guid Id)>(TaskCreationOptions.RunContinuationsAsynchronously);

        service.DeviceFound += (_, device) => deviceFound.TrySetResult(device);

        await service.StartScanningAsync();

        var completed = await Task.WhenAny(deviceFound.Task, Task.Delay(TimeSpan.FromSeconds(2)));

        Assert.Same(deviceFound.Task, completed);
        var device = await deviceFound.Task;
        Assert.Equal("mock rpi0", device.Name);
        Assert.NotEqual(Guid.Empty, device.Id);
    }

    [Fact]
    public async Task ConnectAsync_RaisesSensorDataInExpectedRange()
    {
        var service = new MockBluetoothService();
        var sensorDataReceived = new TaskCompletionSource<SensorData>(TaskCreationOptions.RunContinuationsAsynchronously);

        service.SensorDataReceived += (_, data) => sensorDataReceived.TrySetResult(data);

        await service.ConnectAsync(Guid.NewGuid());

        var completed = await Task.WhenAny(sensorDataReceived.Task, Task.Delay(TimeSpan.FromSeconds(2)));
        await service.DisconnectAsync();

        Assert.Same(sensorDataReceived.Task, completed);

        var data = await sensorDataReceived.Task;
        AssertInRange(data.Ultrasonic1);
        AssertInRange(data.Ultrasonic2);
        AssertInRange(data.Lidar1);
        AssertInRange(data.Lidar2);
        AssertInRange(data.Obstacle);
    }

    private static void AssertInRange(double value)
    {
        Assert.InRange(value, 0.0, 5.0);
    }
}
