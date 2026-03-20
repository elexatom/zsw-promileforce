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

// physcial limits of sensors for data to be reliable
const (
	UsMinReliable  = 3.0   // cm
	UsMaxReliable  = 130.0 // cm
	TofMinReliable = 30    // mm
	TofMaxReliable = 600   // mm
)

// GetJSON Safely locks and updates the sensor data
// Return formatted text
func (d *SensorData) GetJSON() string {
	d.mu.RLock()
	defer d.mu.RUnlock()

	// reliability evaluation
	usReliable := d.Ultrasonic >= UsMinReliable && d.Ultrasonic <= UsMaxReliable
	tofReliable := d.TofMm >= TofMinReliable && d.TofMm <= TofMaxReliable

	return fmt.Sprintf(
		`{"us_cm":%.1f,"us_reliable":%t,"tof_mm":%d,"tof_reliable":%t,"laser_clear":%t}`,
		d.Ultrasonic, usReliable, d.TofMm, tofReliable, d.LaserClear,
	)
}
