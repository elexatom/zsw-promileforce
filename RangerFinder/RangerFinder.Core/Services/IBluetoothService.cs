using RangerFinder.Core.Models;
using System;
using System.Threading.Tasks;

namespace RangerFinder.Core.Services
{
    public interface IBluetoothService
    {
        event EventHandler<SensorData> SensorDataReceived;
        event EventHandler<(string Name, Guid Id)> DeviceFound;

        Task StartScanningAsync();
        Task StopScanningAsync();
        Task ConnectAsync(Guid deviceId);
        Task DisconnectAsync();
    }
}
