using Baetoti.Core.Interface.Services;
using Liphsoft.Crypto.Argon2;
using System;

namespace Baetoti.Infrastructure.Services
{
    public class HashingService : IArgon2Service
    {

        private readonly PasswordHasher _hasher;

        public HashingService()
        {
            _hasher = new PasswordHasher(memoryCost: uint.Parse("65536"));
        }

        public string GenerateHash(string password)
        {
            return _hasher.Hash(password, Guid.NewGuid().ToString());
        }

        public bool VerifyHash(string password, string hashPassword)
        {
            return _hasher.Verify(hashPassword, password);
        }

    }
}
