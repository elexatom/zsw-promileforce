//	Author: Tomas Elexa
//  data.go - defines the SensorData struct and methods for thread-safe access
package main

import (
	"fmt"
	"sync"
)

// SensorData safely stores the latest sensor readings
type SensorData struct {
	mu          sync.RWMutex // for parallel access
	Ultrasonic 	float64
	ToF_mm			uint16
	LaserClear	bool
}

// Safely locks and updates the sensor data
// Return formatted text
func (d *SensorData) GetJSON() string {
	d.mu.RLock()
	defer d.mu.RUnlock()

	return fmt.Sprintf(`{"ultrasonic": %.2f, "tof_mm": %d, "laser_clear": %t}`,
		d.Ultrasonic, d.ToF_mm, d.LaserClear)
}