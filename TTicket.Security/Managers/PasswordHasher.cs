using TTicket.Security.Interfaces;
using System.Security.Cryptography;

namespace TTicket.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private const char Delimeter = ';';

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);
            return string.Join(Delimeter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }
        public bool Verify(string hashedPassword, string inputPassword)
        {
            if (inputPassword == null)
            {
                return false;
            }
            string[] elements = hashedPassword.Split(Delimeter);
            byte[] salt = Convert.FromBase64String(elements[0]);
            byte[] hash = Convert.FromBase64String(elements[1]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, Iterations, _hashAlgorithmName, KeySize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}