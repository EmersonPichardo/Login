using login_web_api.SettingsModels;
using System;
using System.Security.Cryptography;
using System.Text;

namespace login_web_api
{
    public static class PasswordGenerator
    {
        public static (byte[], byte[]) Generate(string password, HashingConfiguration hashingConfiguration)
        {
            byte[] pureSalt = new byte[hashingConfiguration.SaltLength];
            new RNGCryptoServiceProvider().GetNonZeroBytes(pureSalt);

            byte[] interations = BitConverter.GetBytes(hashingConfiguration.Iterations);
            byte[] algorithm = Encoding.UTF8.GetBytes(hashingConfiguration.Algorithm);

            byte[] salt = new byte[hashingConfiguration.SaltLength + interations.Length + algorithm.Length];
            Buffer.BlockCopy(pureSalt, 0, salt, 0, pureSalt.Length);
            Buffer.BlockCopy(interations, 0, salt, pureSalt.Length, interations.Length);
            Buffer.BlockCopy(algorithm, 0, salt, pureSalt.Length + interations.Length, algorithm.Length);

            byte[] hashedPassword = new Rfc2898DeriveBytes(password, salt, hashingConfiguration.Iterations, new HashAlgorithmName(hashingConfiguration.Algorithm)).GetBytes(hashingConfiguration.AlgorithmLength);

            return (hashedPassword, salt);
        }
    }
}
