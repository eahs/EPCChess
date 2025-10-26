
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ADSBackend.Helpers
{
    /// <summary>
    /// Represents a hashed password and its salt.
    /// </summary>
    public class PasswordHash
    {
        /// <summary>
        /// Gets or sets the salt used for hashing.
        /// </summary>
        public string Salt { get; set; }
        /// <summary>
        /// Gets or sets the hashed password.
        /// </summary>
        public string HashedPassword { get; set; }
    }

    /// <summary>
    /// Provides methods for hashing passwords.
    /// </summary>
    public class PasswordHasher
    {
        /// <summary>
        /// Hashes a password with a new, randomly generated salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>A <see cref="PasswordHash"/> object containing the salt and hashed password.</returns>
        public static PasswordHash Hash(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Hash(password, salt);
        }

        /// <summary>
        /// Hashes a password using a provided salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt as a Base64 string.</param>
        /// <returns>A <see cref="PasswordHash"/> object containing the salt and hashed password.</returns>
        public static PasswordHash Hash(string password, string salt)
        {
            byte[] _salt = Convert.FromBase64String(salt);

            return Hash(password, _salt);
        }

        /// <summary>
        /// Hashes a password using a provided salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt as a byte array.</param>
        /// <returns>A <see cref="PasswordHash"/> object containing the salt and hashed password.</returns>
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