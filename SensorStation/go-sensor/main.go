// Author: Tomas Elexa
// Description: Main entry point for the Sensor Station application.
// Initializes Bluetooth and sensor modules, collects data, and sends it at regular intervals.

package main

import (
	"fmt"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/d2r2/go-logger"
)

func main() {
	fmt.Println("[INFO] Starting Sensor Station...")

	err := logger.ChangePackageLogLevel("i2c", logger.InfoLevel)
	if err != nil {
		return
	}

	err = logger.ChangePackageLogLevel("vl53l0x", logger.InfoLevel)
	if err != nil {
		return
	}

	fmt.Println("[INFO] Log level set to INFO for i2c and vl53l0x packages.")

	err = InitCrypto()
	if err != nil {
		fmt.Println("[ERROR] Failed to initialize crypto:", err)
		return
	}

	// data storage
	data := &SensorData{}

	// start modules
	InitBLE()
	fmt.Println("[INFO] Bluetooth module started.")

	cleanup := InitSensors(data)
	defer cleanup() // ensure sensors are properly closed on exit
	fmt.Println("[INFO] Sensor modules started.")

	// main loop
	go func() {
		for {
			payload := data.GetJSON() // get data

			fmt.Printf("[%s] %s\n", time.Now().Format("15:04:05"), payload)

			encryptedPayload := Encrypt(payload)

			SendData(encryptedPayload) // send data

			time.Sleep(200 * time.Millisecond)
		}
	}()

	// wait for main loop to finish and do cleanup
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	fmt.Println("[INFO] Exiting Sensor Station...")
}
