// Author: Tomas Elexa
// Description: This file contains the sensor reading logic for the laser, ultrasonic, and TOF sensors. 
// It initializes the sensors, starts GO routines to continuously read data, and updates the shared SensorData struct with the latest readings. 
// The function returns a cleanup function to close GPIO resources when the program exits.
package main

import (
	"log"
	"time"

	// "github.com/d2r2/go-i2c"
	// "github.com/d2r2/go-vl53l0x"
	"github.com/stianeikeland/go-rpio/v4"
)

// RPi GPIO pin numbers
const LaserPinNum = 22
const TrigPinNum = 23
const EchoPinNum = 24

// Initializes sensors and starts GO routines to read data continuously.
func InitSensors(data *SensorData) func() {
	err := rpio.Open()
	if err != nil {
		log.Fatal(err)
	}

	// ------ GPIO Setup ------
	// Laser sensor setup
	laserPin := rpio.Pin(LaserPinNum)
	laserPin.Input();

	// Ultrasonic sensor setup
	trigPin := rpio.Pin(TrigPinNum)
	trigPin.Output()
	trigPin.Low()

	echoPin := rpio.Pin(EchoPinNum)
	echoPin.Input()

	// TOF sensor setup TODO: Uncomment and configure if using VL53L0X
	// i2cBus, err := i2c.NewI2C(0x29, 1) // default I2C address for VL53L0X
	// if err != nil {
	// 	log.Fatalf("Failed to initialize I2C: %v", err)
	// }
	
	// tofSensor := vl53l0x.NewVl53l0x()
	// err = tofSensor.Reset(i2cBus)
  // if err != nil {
  //    log.Fatal(err)
  // }

	// err = tofSensor.Reset(i2cBus)
  // if err != nil {
  //   log.Fatal(err)
  // }

	// err = tofSensor.Init(i2cBus)
	// if err != nil {
	// 	log.Fatalf("Failed to initialize TOF sensor: %v", err)
	// }
	

	// ------- Start GO Routines for sensors -------
	// GO Routine : Laser Sensor
	go func() {
		for {
			state := laserPin.Read() == rpio.High // true if laser is clear
			data.mu.Lock()
			data.LaserClear = state
			data.mu.Unlock()
			time.Sleep(50 * time.Millisecond)
		}
	}()

	// GO Routine : Ultrasonic Sensor
	go func()  {
		for {
			trigPin.High() // pulse
			time.Sleep(10 * time.Microsecond)
			trigPin.Low()

			// wait for echo start
			timeout := time.Now()
			for echoPin.Read() == rpio.Low && time.Since(timeout) < 100*time.Millisecond {}
			start := time.Now()

			for echoPin.Read() == rpio.High && time.Since(start) < 100*time.Millisecond {}
			duration := time.Since(start)

			// distance calculation
			dist := (duration.Seconds() * 34300) / 2 // speed of sound in cm/s divided by 2 for round trip

			data.mu.Lock()
			data.Ultrasonic = dist
			data.mu.Unlock()
			time.Sleep(100 * time.Millisecond)
		}
	}()

	// GO Routine : TOF Sensor TODO: Uncomment if using VL53L0X
	// if i2cBus != nil {
	// 	go func() {
	// 		for {
	// 			distance, err := tofSensor.ReadRangeSingleMillimeters(i2cBus)
	// 			if err == nil {
	// 				data.mu.Lock()
	// 				data.ToF_mm = distance
	// 				data.mu.Unlock()
	// 			}

	// 			time.Sleep(50 * time.Millisecond)
	// 		}
	// 	}()
	// }

	// Return cleanup function for main.go
	return func() {
		rpio.Close()
		// if i2cBus != nil {
		// 	i2cBus.Close()
		// }
	}
}