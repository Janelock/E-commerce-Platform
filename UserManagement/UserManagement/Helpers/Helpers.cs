using System.Security.Cryptography;
using System;
using System.Text;

namespace UserManagement.Helpers
{
    public class Helpers
    {
        public static string HashPassword(string password)
        {
            // Create a salt (random bytes) to add complexity to the hash
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Create the hash with PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20); // 20 bytes for SHA-1

            // Combine the salt and hash
            byte[] hashBytes = new byte[36]; // 16 (salt) + 20 (hash)
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convert to Base64 and return
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string savedHash, string password)
        {
            // Convert the saved hash back to bytes
            byte[] hashBytes = Convert.FromBase64String(savedHash);

            // Extract the salt from the saved hash
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Compute the hash of the provided password using the same salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Compare the computed hash with the saved hash
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }
            return true;
        }

        
    }
}