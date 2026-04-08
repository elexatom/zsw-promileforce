using RangerFinder.Core.Models;
using System;
using System.Threading.Tasks;

namespace RangerFinder.Core.Services
{
    public class MockBluetoothService : IBluetoothService
    {
        public event EventHandler<SensorData> SensorDataReceived;
        public event EventHandler<(string Name, Guid Id)> DeviceFound;

        private bool _isconnected;
        private bool _isscanning;

        public async Task StartScanningAsync()
        {
            _isscanning = true;
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                if (_isscanning)
                {
                    DeviceFound?.Invoke(this, ("mock rpi0", Guid.NewGuid()));
                }
            });
        }

        public Task StopScanningAsync()
        {
            _isscanning = false;
            return Task.CompletedTask;
        }

        public Task ConnectAsync(Guid deviceId)
        {
            _isconnected = true;
            _ = Task.Run(CreateRandom);
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            _isconnected = false;
            return Task.CompletedTask;
        }

        private async Task CreateRandom()
        {
            var random = new Random();
            while (_isconnected)
            {
                // Generuj data (0.0 az 5.0 metru)
                var data = new SensorData(
                    random.NextDouble() * 5.0,
                    random.NextDouble() * 5.0,
                    random.NextDouble() * 5.0,
                    random.NextDouble() * 5.0,
                    random.NextDouble() * 5.0
                );

                SensorDataReceived?.Invoke(this, data);

                await Task.Delay(200); // 5 refreshes par sekund pro cistsi vizual
            }
        }
    }
}
