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

        [ObservableProperty]
        private SensorData _currentData;

        [ObservableProperty]
        private string _connectionStatus = "Disconnected";

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
                if (SensorHistory.Count > 100)
                {
                    SensorHistory.RemoveAt(0);
                }
                OnPropertyChanged(nameof(AverageObstacle));
            });
        }
    }
}
