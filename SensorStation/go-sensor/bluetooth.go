// Author: Tomas Elexa

package main

import (
	"log"

	"tinygo.org/x/bluetooth"
)

var (
	adapter    = bluetooth.DefaultAdapter
	sensorChar bluetooth.Characteristic
)

// InitBLE Initializes the Bluetooth adapter and sets up the service and characteristics
func InitBLE() {
	err := adapter.Enable()
	if err != nil {
		log.Fatalf("Failed to enable Bluetooth adapter: %v", err)
	}

	err = adapter.AddService(&bluetooth.Service{
		UUID: bluetooth.NewUUID([16]byte{0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef, 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef}),
		Characteristics: []bluetooth.CharacteristicConfig{
			{
				Handle: &sensorChar,
				UUID:   bluetooth.New16BitUUID(0x2A3D),
				Value:  []byte("Waiting for data"), // Initial value
				Flags:  bluetooth.CharacteristicReadPermission | bluetooth.CharacteristicNotifyPermission,
			},
		},
	})

	if err != nil {
		log.Fatalf("Failed to add service: %v", err)
	}

	adv := adapter.DefaultAdvertisement()
	err = adv.Configure(bluetooth.AdvertisementOptions{
		LocalName: "SensorStation",
	})

	if err != nil {
		log.Fatalf("Failed to configure advertisement: %v", err)
	}

	err = adv.Start()
	if err != nil {
		log.Fatalf("Failed to start advertisement: %v", err)
	}
}

// SendData Sends JSON data to the connected Bluetooth client
func SendData(payload string) {
	_, err := sensorChar.Write([]byte(payload))
	if err != nil {
		return
	}
}
