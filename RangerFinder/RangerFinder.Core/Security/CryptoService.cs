using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RangerFinder.Core.Security
{
    internal class CryptoService
    {
        private const string SecretKey = "tomasjesuperklukmamhorad";

        private const string SecretIV = "tajnamyskavec";

        public static string DecryptData(byte[] encryptedBytes)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(SecretKey);
            aesAlg.IV = Encoding.UTF8.GetBytes(SecretIV);

            //make a decrypter
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(encryptedBytes);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            //returns decrypted data as string
            return srDecrypt.ReadToEnd();
        }
    }
}
