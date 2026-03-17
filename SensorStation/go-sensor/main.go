package main

import (
	"fmt"
	"log"
	"sync"
	"time"

	//"github.com/d2r2/go-i2c"
	//"github.com/d2r2/go-vl53l0x"
	"github.com/stianeikeland/go-rpio/v4"
)

type SensorData struct {
	Ultrasonic float64
	ToF_mm uint16
	LaserClear bool
	mu sync.Mutex
}

func main() {
	fmt.Println("Starting Sensor Station...")

	// open gpio
	err := rpio.Open()
	if err != nil {
		log.Fatalf("Unable to open GPIO: %v", err)
	}
	defer rpio.Close()

	// gpio setup
	laserPin := rpio.Pin(22) // GPIO17 for laser
	laserPin.Input();

	trigPin := rpio.Pin(23) // GPIO27 for ultrasonic trigger
	trigPin.Output()
	trigPin.Low()

	echoPin := rpio.Pin(24)
	echoPin.Input()

	// TODO: TOF sensor setup

	data := SensorData{}

	fmt.Println("Sensor Station ready. Parallel data collection ongoing.")

	// GO Routine : Laser Sensor
	go func() {
		for {
			state := laserPin.Read() == rpio.High // TODO : check if this logic is correct for laser clear detection
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

			// vypocet vzdalenosti
			dist := (duration.Seconds() * 34300) / 2 // rychlost zvuku v cm/s

			data.mu.Lock()
			data.Ultrasonic = dist
			data.mu.Unlock()
			time.Sleep(100 * time.Millisecond)
		}
	}()

	// GO Routine : ToF Sensor
	// TODO: implement

	// Main loop
	for {
		data.mu.Lock()
		laserStatus := "Obstacle Detected"
		if data.LaserClear {
			laserStatus = "Clear"
		}

		fmt.Printf("Ultrasonic: %.2f cm | ToF: n/a | Laser: %s\n", data.Ultrasonic, laserStatus)
		data.mu.Unlock()
		time.Sleep(100 * time.Millisecond)
	}
}