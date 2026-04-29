using System;

namespace RangerFinder.Core.Security
{
    public static class Secrets
    {
        public static readonly string? EncryptionKey = Environment.GetEnvironmentVariable("key"); 
    }
}
