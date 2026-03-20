//		Author: Tomas Elexa
//	 data.go - defines the SensorData struct and methods for thread-safe access
package main

import (
	"fmt"
	"sync"
)

// SensorData safely stores the latest sensor readings
type SensorData struct {
	mu         sync.RWMutex // for parallel access
	Ultrasonic float64      `json:"ultrasonic,omitempty"`
	TofMm      uint16       `json:"tof_mm,omitempty"`
	LaserClear bool         `json:"laser_clear,omitempty"`
}

// GetJSON Safely locks and updates the sensor data
// Return formatted text
func (d *SensorData) GetJSON() string {
	d.mu.RLock()
	defer d.mu.RUnlock()

	return fmt.Sprintf(`{"ultrasonic": %.2f, "tof_mm": %d, "laser_clear": %t}`,
		d.Ultrasonic, d.TofMm, d.LaserClear)
}
