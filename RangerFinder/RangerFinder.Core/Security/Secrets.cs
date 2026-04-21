using System;

namespace RangerFinder.Core.Security
{
    public static class Secrets
    {
        // Must be exactly 32 bytes for AES-256 (matches ENCRYPTION_SECRET in Go)
        public const string EncryptionKey = "1ba426e89d017cadacd762f2bea6b699"; 
    }
}
