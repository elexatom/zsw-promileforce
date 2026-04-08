using System;

namespace RangerFinder.ViewModels
{
    public class DiscoveredDevice
    {
        public string Name { get; set; } = string.Empty;
        public Guid DeviceId { get; set; }
        public bool IsMock { get; set; }
    }
}
