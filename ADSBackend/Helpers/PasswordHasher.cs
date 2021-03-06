﻿using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ADSBackend.Helpers
{
    public class PasswordHash
    {
        public string Salt { get; set; }
        public string HashedPassword { get; set; }
    }

    public class PasswordHasher
    {
        public static PasswordHash Hash(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Hash(password, salt);
        }

        public static PasswordHash Hash(string password, string salt)
        {
            byte[] _salt = Convert.FromBase64String(salt);

            return Hash(password, _salt);
        }

        public static PasswordHash Hash(string password, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return new PasswordHash
            {
                Salt = Convert.ToBase64String(salt),
                HashedPassword = hashed
            };
        }
    }
}