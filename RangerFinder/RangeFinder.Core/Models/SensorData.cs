using System;
using System.Collections.Generic;
using System.Text;

namespace RangeFinder.Core.Models
{
    public record SensorData(
        double Ultrasonic1,
        double Ultrasonic2,
        double Lidar1,
        double Lidar2,
        double Obstacle
    );
}
