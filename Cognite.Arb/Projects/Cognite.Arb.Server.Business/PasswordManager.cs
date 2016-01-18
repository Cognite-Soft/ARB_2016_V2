using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cognite.Arb.Server.Business
{
    public class PasswordManager : IPasswordManager
    {
        private readonly IConfiguration _configuration;

        public PasswordManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ValidatePasswordStrength(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            return true;
        }

        public bool ValidateSecurePhraseStrength(string securePhrase)
        {
            return securePhrase.Length > 8;
        }

        public string GenerateSalt()
        {
            var randomGenerator = new RNGCryptoServiceProvider();
            var salt = new byte[sizeof (char)*16];
            randomGenerator.GetBytes(salt);
            var saltString = Convert.ToBase64String(salt);
            return saltString;
        }

        public string HashPassword(string password, string salt)
        {
            var passwordBytes = Encoding.Unicode.GetBytes(password + salt);
            var algorithm = new SHA256Managed();
            var hash = algorithm.ComputeHash(passwordBytes);
            var result = Convert.ToBase64String(hash);
            return result;
        }

        public string EncryptSecurePhrase(string phrase)
        {
            var input = Encoding.Unicode.GetBytes(phrase);
            var key = _configuration.SecurePhraseEncryptionKey;
            var iv = _configuration.SecurePhraseIv;

            using (var des = CreateDes())
            {
                using (var encryptor = des.CreateEncryptor(key, iv))
                {
                    var output = encryptor.TransformFinalBlock(input, 0, input.Length);
                    return Convert.ToBase64String(output);
                }
            }
        }

        public string DecryptSecurePhrase(string phrase)
        {
            var input = Convert.FromBase64String(phrase);
            var key = _configuration.SecurePhraseEncryptionKey;
            var iv = _configuration.SecurePhraseIv;

            using (var des = CreateDes())
            {
                using (var decryptor = des.CreateDecryptor(key, iv))
                {
                    var output = decryptor.TransformFinalBlock(input, 0, input.Length);
                    return Encoding.Unicode.GetString(output);
                }
            }
        }

        private static DES CreateDes()
        {
            var des = DES.Create();
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.ANSIX923;
            return des;
        }

        public Tuple<int, int> GetSecurePhraseQuestion(int length)
        {
            if (length < 2) throw new Exception("Secure Phrase Length is less than 2 symbols");
            var random = new Random((int) DateTime.Now.Ticks);
            int first = random.Next(length);
            var delta = random.Next(length - 1) + 1;
            int second = first + delta;
            if (second >= length) second -= length;
            return new Tuple<int, int>(Math.Min(first, second), Math.Max(first, second));
        }
    }
}