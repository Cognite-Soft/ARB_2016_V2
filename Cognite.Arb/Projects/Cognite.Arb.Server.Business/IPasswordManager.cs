using System;

namespace Cognite.Arb.Server.Business
{
    public interface IPasswordManager
    {
        bool ValidatePasswordStrength(string password);
        bool ValidateSecurePhraseStrength(string securePhrase);
        string GenerateSalt();
        string HashPassword(string password, string salt);
        string EncryptSecurePhrase(string phrase);
        string DecryptSecurePhrase(string phrase);
        Tuple<int, int> GetSecurePhraseQuestion(int length);
    }
}
