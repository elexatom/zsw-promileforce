using System;

namespace RangerFinder.ViewModels
{
    public class DiscoveredDevice
    {
        public string Name { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public bool IsMock { get; set; }
    }
}
