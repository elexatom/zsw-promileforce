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

        public MainViewModel(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
            _bluetoothService.SensorDataReceived += OnSensorDataReceived;
        }

        [RelayCommand]
        private async Task ConnectAsync()
        {
            ConnectionStatus = "Connecting...";
            await _bluetoothService.ConnectAsync();
            ConnectionStatus = "Connected";
        }

        [RelayCommand]
        private async Task DisconnectAsync()
        {
            await _bluetoothService.DisconnectAsync();
            ConnectionStatus = "Disconnected";
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
            });
        }
    }
}
