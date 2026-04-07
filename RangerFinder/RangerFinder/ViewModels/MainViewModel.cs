using System;
using System.Collections.Generic;
using System.Text;
using RangerFinder.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RangerFinder.Core.Models;

namespace RangerFinder.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IBluetoothService _bluetoothService;

        public System.Collections.ObjectModel.ObservableCollection<SensorData> SensorHistory { get; } = new();
        public System.Collections.ObjectModel.ObservableCollection<string> ConsoleLogs { get; } = new();
        private DateTime _lastLogTime = DateTime.MinValue;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MinDistance))]
        [NotifyPropertyChangedFor(nameof(MaxDistance))]
        private SensorData _currentData;

        public double MinDistance
        {
            get
            {
                if (CurrentData == null) return 0;
                double min = CurrentData.Ultrasonic1;
                if (CurrentData.Ultrasonic2 < min) min = CurrentData.Ultrasonic2;
                if (CurrentData.Lidar1 < min) min = CurrentData.Lidar1;
                if (CurrentData.Lidar2 < min) min = CurrentData.Lidar2;
                return min;
            }
        }

        public double MaxDistance
        {
            get
            {
                if (CurrentData == null) return 0;
                double max = CurrentData.Ultrasonic1;
                if (CurrentData.Ultrasonic2 > max) max = CurrentData.Ultrasonic2;
                if (CurrentData.Lidar1 > max) max = CurrentData.Lidar1;
                if (CurrentData.Lidar2 > max) max = CurrentData.Lidar2;
                return max;
            }
        }

        [ObservableProperty]
        private string _connectionStatus = "Disconnected";

        [ObservableProperty]
        private string _connectedDeviceName = "Unknown Device";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotConnected))]
        [NotifyPropertyChangedFor(nameof(ConnectButtonText))]
        private bool _isConnected;

        [ObservableProperty]
        private bool _isScanning;

        public System.Collections.ObjectModel.ObservableCollection<DiscoveredDevice> DiscoveredDevices { get; } = new();

        public bool IsNotConnected => !IsConnected;
        public string ConnectButtonText => IsConnected ? "DISCONNECT" : (IsScanning ? "SCANNING..." : "CONNECT");

        public double AverageObstacle => SensorHistory.Count == 0 ? 0 : SensorHistory.Average(s => s.Obstacle);

        public MainViewModel(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
            _bluetoothService.SensorDataReceived += OnSensorDataReceived;
        }

        [RelayCommand]
        private async Task ToggleConnectionAsync()
        {
            if (IsConnected)
            {
                await DisconnectAsync();
            }
            else
            {
                if (IsScanning)
                {
                    // TODO Backend: StopScanningAsync()
                    IsScanning = false;
                    OnPropertyChanged(nameof(ConnectButtonText));
                    return;
                }

                IsScanning = true;
                OnPropertyChanged(nameof(ConnectButtonText));
                
                DiscoveredDevices.Clear();
                // Permanent fallback option
                DiscoveredDevices.Add(new DiscoveredDevice { Name = "mock rpi0", MacAddress = "00:00:00:00:00:00", IsMock = true });

                // TODO Backend: zavolat _bluetoothService.StartScanningAsync()
                // TODO Backend: zaregistrovat event _bluetoothService.DeviceFound += OnDeviceFound
                // A jakmile přijde event, udělat MainThread.BeginInvokeOnMainThread(() => DiscoveredDevices.Add(device));
            }
        }

        [RelayCommand]
        private async Task SelectDeviceAsync(DiscoveredDevice device)
        {
            if (device == null) return;
            
            // TODO Backend: StopScanningAsync()
            IsScanning = false;
            OnPropertyChanged(nameof(ConnectButtonText));
            ConnectedDeviceName = device.Name;

            // TODO Backend: upravit _bluetoothService.ConnectAsync() aby přijímalo device.MacAddress nebo device instanci
            await ConnectAsync();
        }

        [RelayCommand]
        private async Task ConnectAsync()
        {
            ConnectionStatus = "Connecting...";
            await _bluetoothService.ConnectAsync();
            ConnectionStatus = "Connected";
            IsConnected = true;
        }

        [RelayCommand]
        private async Task DisconnectAsync()
        {
            await _bluetoothService.DisconnectAsync();
            ConnectionStatus = "Disconnected";
            IsConnected = false;
        }

        [RelayCommand]
        private async Task OpenSettingsAsync()
        {
            await Shell.Current.GoToAsync(nameof(DebugPage));
        }

        [RelayCommand]
        private async Task OpenDetailsAsync()
        {
            await Shell.Current.GoToAsync(nameof(DetailsPage));
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void OnSensorDataReceived(object sender, SensorData e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CurrentData = e;
                SensorHistory.Add(e);
                if (SensorHistory.Count > 30)
                {
                    SensorHistory.RemoveAt(0);
                }

                if ((DateTime.Now - _lastLogTime).TotalMilliseconds > 200)
                {
                    _lastLogTime = DateTime.Now;
                    string logLine = $"[RX_PKT] OBS: {e.Obstacle:F2}m | LIDAR1: {e.Lidar1:F2}m | US1: {e.Ultrasonic1:F2}m";
                    ConsoleLogs.Insert(0, logLine);
                    if (ConsoleLogs.Count > 30)
                    {
                        ConsoleLogs.RemoveAt(ConsoleLogs.Count - 1);
                    }
                }

                OnPropertyChanged(nameof(AverageObstacle));
            });
        }
    }
}
