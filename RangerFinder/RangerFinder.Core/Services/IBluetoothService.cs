using RangerFinder.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RangerFinder.Core.Services
{
    internal interface IBluetoothService
    {
        event EventHandler<SensorData> SensorDataReceived;

        Task ConnectAsync();
        Task DisconnectAsync();
    }
}
