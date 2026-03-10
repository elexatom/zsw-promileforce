using RangeFinder.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RangeFinder.Core.Services
{
    internal interface IBluetoothService
    {
        event EventHandler<SensorData> DataReceived;

        Task ConnectAsync();
        Task DisconnectAsync();
    }
}
