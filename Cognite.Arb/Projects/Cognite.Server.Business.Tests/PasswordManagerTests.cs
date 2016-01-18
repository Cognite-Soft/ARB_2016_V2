using System;
using Cognite.Arb.Server.Business;
using Moq;
using NUnit.Framework;

namespace Cognite.Server.Business.Tests
{
    [TestFixture]
    public class PasswordManagerTests
    {
        [Test]
        public void HashPasswordReturnsConsistentResult()
        {
            const string password = "test password";
            var passwordManager = new PasswordManager(null);
            var salt = passwordManager.GenerateSalt();
            var hash1 = passwordManager.HashPassword(password, salt);
            var hash2 = passwordManager.HashPassword(password, salt);
            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void HashPasswordReturns44Chars()
        {
            const string password = "test password";
            var passwordManager = new PasswordManager(null);
            var salt = passwordManager.GenerateSalt();
            var hash = passwordManager.HashPassword(password, salt);
            Console.WriteLine(hash);
            Assert.AreEqual(44, hash.Length);
        }

        [Test]
        public void GenerateSaltReturns44Chars()
        {
            var passwordManager = new PasswordManager(null);
            var salt = passwordManager.GenerateSalt();
            Console.WriteLine(salt);
            Assert.AreEqual(44, salt.Length);
        }

        [Test]
        public void GenerateSecurePhraseQuestionOfLength2NeverReturnsTwoSameCharacters()
        {
            var passwordManager = new PasswordManager(null);
            for (int i = 0; i < 1000; ++i)
            {
                var question = passwordManager.GetSecurePhraseQuestion(2);
                Assert.AreEqual(0, question.Item1);
                Assert.AreEqual(1, question.Item2);
            }
        }

        [Test]
        public void GenerateSecurePhraseQuestionNeverReturnsTwoSameCharacters()
        {
            var passwordManager = new PasswordManager(null);
            for (int length = 3; length < 11; ++length)
            {
                for (int i = 0; i < 1000; ++i)
                {
                    var question = passwordManager.GetSecurePhraseQuestion(length);
                    Assert.AreNotEqual(question.Item1, question.Item2);
                    Assert.Greater(question.Item2, question.Item1);
                }
            }
        }

        [Test]
        public void EncryptSecurePhrase_qqqqqqqqq()
        {
            const string securePhrase = "qqqqqqqqq";

            var mockRepository = new Moq.MockRepository(MockBehavior.Strict);
            var configurationMock = mockRepository.Create<IConfiguration>();
            configurationMock.SetupGet(x => x.SecurePhraseEncryptionKey).Returns(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            configurationMock.SetupGet(x => x.SecurePhraseIv).Returns(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            var configuration = configurationMock.Object;

            var passwordManager = new PasswordManager(configuration);
            var encrypted = passwordManager.EncryptSecurePhrase(securePhrase);
            var decrypted = passwordManager.DecryptSecurePhrase(encrypted);

            Assert.AreEqual(securePhrase, decrypted);
        }
    }
}
