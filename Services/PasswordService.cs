using System.Security.Cryptography;
using System.Text;

namespace Software_Engineering_2025.Services
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            string hashedInput = HashPassword(password);
            return hashedInput == passwordHash;
        }
    }
}