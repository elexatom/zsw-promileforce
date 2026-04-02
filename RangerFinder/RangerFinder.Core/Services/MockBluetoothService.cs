using RangerFinder.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RangerFinder.Core.Services
{
    public class MockBluetoothService : IBluetoothService
    {
        public event EventHandler<SensorData> SensorDataReceived;
        private bool _isconnected;

        public Task ConnectAsync()
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
