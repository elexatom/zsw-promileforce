// Author: Tomas Elexa

package env

import "os"

// Key represents an environment variable key
type Key string

// GetValue returns the value of the environment variable
func (key Key) GetValue() string {
	return os.Getenv(string(key))
}

const (
	EncryptionSecret Key = "ENCRYPTION_SECRET" // Secret key for AES encryption
)
