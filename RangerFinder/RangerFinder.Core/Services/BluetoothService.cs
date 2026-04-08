using RangerFinder.Core.Models;
using RangerFinder.Core.Security;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System.Text;

namespace RangerFinder.Core.Services
{
    public class BluetoothService : IBluetoothService
    {
        public event EventHandler<SensorData> SensorDataReceived;
        public event EventHandler<(string Name, Guid Id)> DeviceFound;

        private readonly IAdapter _adapter;
        private IDevice _device;
        private ICharacteristic _sensorChar;

        private static readonly Guid ServiceUuid = new Guid("12345678-90ab-cdef-1234-567890abcdef");
        private static readonly Guid CharacteristicUuid = new Guid("00002A3D-0000-1000-8000-00805f9b34fb");

        public BluetoothService()
        {
            _adapter = CrossBluetoothLE.Current.Adapter;
            _adapter.DeviceDiscovered += OnDeviceDiscovered;
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs a)
        {
            var name = string.IsNullOrWhiteSpace(a.Device.Name) ? "Unknown" : a.Device.Name;
            DeviceFound?.Invoke(this, (name, a.Device.Id));
        }

        public async Task StartScanningAsync()
        {
            await _adapter.StartScanningForDevicesAsync();
        }

        public async Task StopScanningAsync()
        {
            await _adapter.StopScanningForDevicesAsync();
        }

        public async Task ConnectAsync(Guid deviceId)
        {
            _device = await _adapter.ConnectToKnownDeviceAsync(deviceId);

            if (_device != null)
            {
                var service = await _device.GetServiceAsync(ServiceUuid);
                if (service != null)
                {
                    _sensorChar = await service.GetCharacteristicAsync(CharacteristicUuid);
                    if (_sensorChar != null)
                    {
                        _sensorChar.ValueUpdated += OnCharacteristicValueUpdated;
                        await _sensorChar.StartUpdatesAsync();
                    }
                }
            }
        }

        private void OnCharacteristicValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            try
            {
                string base64Payload = Encoding.UTF8.GetString(e.Characteristic.Value);
                if (string.IsNullOrWhiteSpace(base64Payload)) return;

                string json = CryptoService.DecryptData(base64Payload);
                var node = JsonNode.Parse(json);

                if (node != null)
                {
                    float usCm = node["us_cm"]?.GetValue<float>() ?? 0f;
                    int tofMm = node["tof_mm"]?.GetValue<int>() ?? 0;
                    bool laserClear = node["laser_clear"]?.GetValue<bool>() ?? false;

                    var parsedData = new SensorData(
                        Ultrasonic1: usCm / 100.0,
                        Ultrasonic2: 0.0,
                        Lidar1: tofMm / 1000.0,
                        Lidar2: 0.0,
                        Obstacle: laserClear ? 5.0 : 0.0
                    );

                    SensorDataReceived?.Invoke(this, parsedData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BLE ERR] {ex.Message}");
            }
        }

        public async Task DisconnectAsync()
        {
            if (_sensorChar != null)
            {
                try { await _sensorChar.StopUpdatesAsync(); } catch { }
                _sensorChar.ValueUpdated -= OnCharacteristicValueUpdated;
                _sensorChar = null;
            }

            if (_device != null)
            {
                await _adapter.DisconnectDeviceAsync(_device);
                _device = null;
            }
        }
    }
}
