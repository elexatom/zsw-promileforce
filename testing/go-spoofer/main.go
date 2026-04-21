package main

import (
	"crypto/aes"
	"crypto/cipher"
	"crypto/rand"
	"encoding/base64"
	"fmt"
	"io"
	"log"
	"math"
	"os"
	"os/signal"
	"syscall"
	"time"

	"tinygo.org/x/bluetooth"
)

var adapter = bluetooth.DefaultAdapter
var sensorChar bluetooth.Characteristic

var SecretKey = []byte("1ba426e89d017cadacd762f2bea6b699") // Totožný klíč jako v Secrets.cs

func Encrypt(plaintext string) string {
	block, err := aes.NewCipher(SecretKey)
	if err != nil {
		log.Fatal("[ERROR] Encryption failed: ", err)
		return ""
	}

	aesgcm, err := cipher.NewGCM(block)
	if err != nil {
		log.Fatal("[ERROR] Encryption (GCM) failed: ", err)
		return ""
	}

	// generate a random nonce for each packet (12 bytů pro GCM)
	nonce := make([]byte, aesgcm.NonceSize())
	if _, err := io.ReadFull(rand.Reader, nonce); err != nil {
		log.Fatal("[ERROR] Encryption (nonce) failed: ", err)
		return ""
	}

	// encrypt; nonce is prepended to ciphertext internally exactly like the real app 
	ciphertext := aesgcm.Seal(nonce, nonce, []byte(plaintext), nil)

	// convert to base64 string
	return base64.StdEncoding.EncodeToString(ciphertext)
}

func InitBLE() error {
	err := adapter.Enable()
	if err != nil {
		return fmt.Errorf("failed to enable Bluetooth adapter: %w", err)
	}

	err = adapter.AddService(&bluetooth.Service{
		UUID: bluetooth.NewUUID([16]byte{0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef, 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef}),
		Characteristics: []bluetooth.CharacteristicConfig{
			{
				Handle: &sensorChar,
				UUID:   bluetooth.New16BitUUID(0x2A3D),
				Value:  []byte("Waiting for data"),
				Flags:  bluetooth.CharacteristicReadPermission | bluetooth.CharacteristicNotifyPermission,
			},
		},
	})

	if err != nil {
		return fmt.Errorf("failed to add service: %w", err)
	}

	adv := adapter.DefaultAdvertisement()
	err = adv.Configure(bluetooth.AdvertisementOptions{
		LocalName: "SensorStation", 
	})

	if err != nil {
		return fmt.Errorf("failed to configure advertisement: %w", err)
	}

	err = adv.Start()
	if err != nil {
		return fmt.Errorf("failed to start advertisement: %w", err)
	}

	return nil
}

func main() {
	fmt.Println("[INFO] Starting native Go Spoofer pro Windows...")

	err := InitBLE()
	if err != nil {
		log.Fatalf("[ERROR] BLE error: %v (Máš zapnutý Bluetooth ve Windows?)", err)
	}
	fmt.Println("[INFO] Bluetooth server běží a vydává se za 'SensorStation'!")

	go func() {
		t := 0.0
		for {
			// Simulace nějakého pohybu senzoru vpřed a vzad
			usCm := 50.0 + 30.0*math.Sin(t)
			tofMm := 1200 + 800*math.Cos(t)
			laserClear := usCm > 30.0

			payload := fmt.Sprintf(`{"us_cm":%.1f,"us_reliable":true,"tof_mm":%d,"tof_reliable":true,"laser_clear":%t}`, usCm, int(tofMm), laserClear)
			
			// Magie s pravým AES klíčem
			encryptedPayload := Encrypt(payload)

			fmt.Printf("[%s] Posílám AES Payload -> US: %.1f cm, TOF: %d mm, CLEAR: %t\n", time.Now().Format("15:04:05"), usCm, int(tofMm), laserClear)

			_, err := sensorChar.Write([]byte(encryptedPayload))
			if err != nil {
				// Může zlobit, pokud není nikdo připojený apod.
			}

			time.Sleep(500 * time.Millisecond) // pošleme každé půl vteřiny
			t += 0.2
		}
	}()

	// Čekání na ukončení přes CTRL+C
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	fmt.Println("\n[INFO] Ukončuji Go Spoofer...")
}
