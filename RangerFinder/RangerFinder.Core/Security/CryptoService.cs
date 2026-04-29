using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RangerFinder.Core.Security
{
    internal class CryptoService
    {
        public static string DecryptData(string base64Payload)
        {
            if (string.IsNullOrEmpty(base64Payload))
                return string.Empty;

            byte[] encryptedBytes = Convert.FromBase64String(base64Payload);
            return DecryptData(encryptedBytes);
        }

        public static string DecryptData(byte[] payload)
        {
            if (payload == null || payload.Length < 28) // 12 nonce + 16 tag minimum
            {
                throw new ArgumentException("Payload is too short to be a valid AES-GCM packet.");
            }

            if (Secrets.EncryptionKey == null)
                throw new ArgumentException("Encryption key is not set.");

            byte[] keyBytes = Encoding.UTF8.GetBytes(Secrets.EncryptionKey);
            if (keyBytes.Length != 32)
                throw new ArgumentException("Key must be exactly 32 bytes for AES-256.");

            // Extract components
            int nonceSize = 12; // GCM default
            int tagSize = 16;   // GCM default
            int cipherSize = payload.Length - nonceSize - tagSize;

            byte[] nonce = new byte[nonceSize];
            byte[] ciphertext = new byte[cipherSize];
            byte[] tag = new byte[tagSize];

            Buffer.BlockCopy(payload, 0, nonce, 0, nonceSize);
            Buffer.BlockCopy(payload, nonceSize, ciphertext, 0, cipherSize);
            Buffer.BlockCopy(payload, nonceSize + cipherSize, tag, 0, tagSize);

            byte[] decryptedData = new byte[cipherSize];

            using (var aesGcm = new AesGcm(keyBytes, tagSize))
            {
                // AesGcm handles the decryption and validation
                aesGcm.Decrypt(nonce, ciphertext, tag, decryptedData);
            }

            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}
