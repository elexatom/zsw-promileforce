using RangerFinder.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RangerFinder.Core.Services
{
    public interface IBluetoothService
    {
        event EventHandler<SensorData> SensorDataReceived;

        Task ConnectAsync();
        Task DisconnectAsync();
    }
}
