using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace backend.Helpers
{
    // Source: https://stackoverflow.com/questions/4181198/how-to-hash-a-password
    public class SecurePasswordHasher
    {
        private readonly int saltSize;
        private readonly int hashSize;

        public SecurePasswordHasher(IOptions<HasherSettings> hasherSettings)
        {
            this.saltSize = hasherSettings.Value.SaltSize;
            this.hashSize = hasherSettings.Value.HashSize;
        }

        public string Hash(string password, int iterations)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[this.saltSize]);

            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(this.hashSize);

            // Combine salt and hash
            var hashBytes = new byte[this.saltSize + this.hashSize];
            Array.Copy(salt, 0, hashBytes, 0, this.saltSize);
            Array.Copy(hash, 0, hashBytes, this.saltSize, this.hashSize);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return string.Format("$MYHASH$V1${0}${1}", iterations, base64Hash);
        }

        public string Hash(string password)
        {
            return Hash(password, 10000);
        }

        public bool IsHashSupported(string hashString)
        {
            return hashString.Contains("$MYHASH$V1$");
        }

        public bool Verify(string password, string hashedPassword)
        {
            // Check hash
            if (!IsHashSupported(hashedPassword))
            {
                throw new NotSupportedException("The hashtype is not supported");
            }

            // Extract iteration and Base64 string
            var splittedHashString = hashedPassword.Replace("$MYHASH$V1$", "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];

            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Get salt
            var salt = new byte[this.saltSize];
            Array.Copy(hashBytes, 0, salt, 0, this.saltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(this.hashSize);

            // Get result
            for (var i = 0; i < this.hashSize; i++)
            {
                if (hashBytes[i + this.saltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
