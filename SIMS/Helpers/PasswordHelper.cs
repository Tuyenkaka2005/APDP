using System.Security.Cryptography;
using System.Text;

namespace SIMS.Helpers
{
    public static class PasswordHelper
    {
        public static byte[] HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                return sha256.ComputeHash(bytes);
            }
        }

        public static bool VerifyPassword(string password, byte[] storedHash)
        {
            byte[] hashToCompare = HashPassword(password);
            return hashToCompare.SequenceEqual(storedHash);
        }
    }
}