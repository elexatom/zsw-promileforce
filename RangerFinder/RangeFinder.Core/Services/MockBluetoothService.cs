using RangeFinder.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RangeFinder.Core.Services
{
    internal class MockBluetoothService : IBluetoothService
    {
        public event EventHandler<SensorData> DataReceived;
        private bool _isConnected;

        public async Task ConnectAsync()
        {
            _isConnected = true;

            _ = GenerateRandomAsync();

            await Task.CompletedTask;
        }

        public async Task DisconnectAsync()
        {
            _isConnected = false;
            await Task.CompletedTask;
        }

        private async Task GenerateRandomAsync()
        {
            var random = new Random();

            while (_isConnected)
            {
                var data = new SensorData(
                    random.Next(10, 25),  // Ultrasonic1
                    random.Next(10, 25),  // Ultrasonic2
                    random.Next(10, 25),  // Lidar1
                    random.Next(10, 25),  // Lidar2
                    random.Next(0, 2)     // Obstacle
                );

                DataReceived?.Invoke(this, data);

                await Task.Delay(1000); // Simulate data every second
            }
        }
    }
}
