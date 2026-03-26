using System;
using System.Security.Cryptography;

namespace PasswordHasherApp
{
    class Program
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        static void Main(string[] args)
        {
            string password = "Password123!";
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);
            Console.WriteLine($"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}");
        }
    }
}
