// Author: Tomas Elexa

package main

import (
	"crypto/aes"
	"crypto/cipher"
	"crypto/rand"
	"encoding/base64"
	"fmt"
	"go-sensor/env"
	"io"
	"log"
)

var SecretKey []byte

// InitCrypto initializes the AES-256 encryption key
func InitCrypto() error {
	key := env.EncryptionSecret.GetValue()

	if key == "" {
		return fmt.Errorf("[ERROR] Environment variable %s is not set", env.EncryptionSecret)
	}

	if len(key) != 32 {
		return fmt.Errorf("AES-256 key must have exactly 32 chars (current has %d)", len(key))
	}

	SecretKey = []byte(key)
	fmt.Println("[INFO] AES-256 key loaded")
	return nil
}

// Encrypt encrypts plaintext using AES encryption
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

	// generate a random nonce for each packet
	nonce := make([]byte, aesgcm.NonceSize())
	if _, err := io.ReadFull(rand.Reader, nonce); err != nil {
		log.Fatal("[ERROR] Encryption (nonce) failed: ", err)
		return ""
	}

	// encrypt; nonce is prepended to ciphertext
	ciphertext := aesgcm.Seal(nonce, nonce, []byte(plaintext), nil)

	// convert to base64 string
	return base64.StdEncoding.EncodeToString(ciphertext)
}
