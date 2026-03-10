using RangerFinder.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RangerFinder.Core.Services
{
    internal class MockBluetoothService : IBluetoothService
    {
        public event EventHandler<SensorData> SensorDataReceived;
        private bool _isconnected;

        public async Task ConnectAsync()
        {
            _isconnected = true;

            _ = CreateRandom();

            await Task.CompletedTask;
        }

        public async Task DisconnectAsync()
        {
            _isconnected = false;

            await Task.CompletedTask;
        }

        private async Task CreateRandom()
        {
            var random = new Random();
            while (_isconnected)
            {
                var data = new SensorData(
                    random.Next(1, 25),
                    random.Next(1, 25),
                    random.Next(1, 25),
                    random.Next(1, 25),
                    random.Next(1, 2)
                );
            }

            await Task.Delay(1000);
        }
    }
}
