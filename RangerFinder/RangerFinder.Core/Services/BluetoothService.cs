using RangerFinder.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace RangerFinder.Core.Services
{
    internal class BluetoothService : IBluetoothService
    {
        public event EventHandler<SensorData> SensorDataReceived;

        private readonly IAdapter _adapter;
        private IDevice _device;

        public BluetoothService()
        {
            _adapter = CrossBluetoothLE.Current.Adapter;
        }

        public async Task ConnectAsync()
        {
            _adapter.DeviceDiscovered += (s, a) =>
            {
                if (a.Device.Name == "rpi0")
                {
                    //found the device
                    _device = a.Device;
                }
            };

            await _adapter.StartScanningForDevicesAsync();

            if (_device != null)
            {
                await _adapter.ConnectToDeviceAsync(_device); // pripojeno i guess

                //TODO dopsat cteni dat
            }
        }

        public async Task DisconnectAsync()
        {
            if (_device != null)
            {
                await _adapter.DisconnectDeviceAsync(_device);
            }
        }
    }
}
