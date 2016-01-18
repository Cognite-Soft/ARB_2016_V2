using System;
using System.Configuration;
using Cognite.Arb.Server.Contract;
using NUnit.Framework;

namespace Cognite.Arb.Web.ServiceClient.Tests
{
    [TestFixture]
    public class DefaultUser
    {
        public const string Email = WebApiClient.DefaultUserMail;
        public const string Password = "password";
        public const string Phrase = "securephrase";

        public static WebApiClient CreateWebApiClient()
        {
            string webApiAddress = ConfigurationManager.AppSettings["WebApiUrl"];
            return new WebApiClient(webApiAddress);
        }

        public static User Reset()
        {
            var client = CreateWebApiClient();
            return client.ResetDefaultUser(Password, Phrase);
        }

        public static AuthenticationResult Login()
        {
            return UserManagementTests.Login(Email, Password, Phrase);
        }

        [SetUp]
        public void Setup()
        {
            Database.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            Database.Clear();
        }

        [Test]
        public void CanCreateDefaultUser()
        {
            var user = DefaultUser.Reset();
            var login = DefaultUser.Login();

            var client = CreateWebApiClient();
            var users = client.GetUsers(login.SecurityToken);

            Assert.AreEqual(DefaultUser.Email, user.Email);
            Assert.AreEqual(1, users.Length);
            Assert.AreEqual(user.Id, users[0].Id);
            Assert.AreEqual(user.Email, users[0].Email);
            Assert.AreEqual(user.FirstName, users[0].FirstName);
            Assert.AreEqual(user.LastName, users[0].LastName);
            Assert.AreEqual(user.Role, users[0].Role);
        }

        [Test]
        public void CanResetDefaultUser()
        {
            DefaultUser.Reset();
            var login = DefaultUser.Login();

            var client = CreateWebApiClient();
            var users = client.GetUsers(login.SecurityToken);
            Assert.AreEqual(1, users.Length);

            const string password = "password123";
            const string phrase = "phrase123";

            client.ResetDefaultUser(password, phrase);

            ExpectException.From<InvalidUserOrPasswordException>(() => DefaultUser.Login());

            var question = client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, password);
            var login2 = client.FinishLoginWithSecurePhraseAnswer(question.SecurityToken,
                new SecurePhraseAnswer
                {
                    FirstCharacter = phrase[question.FirstCharacterIndex],
                    SecondCharacter = phrase[question.SecondCharacterIndex],
                });

            var users2 = client.GetUsers(login2.SecurityToken);
            Assert.AreEqual(1, users2.Length);
        }

        [Test, Explicit]
        public void StsStressTest()
        {
            DefaultUser.Reset();
            var login = DefaultUser.Login();

            var client = CreateWebApiClient();
            for (int i = 0; i < 1000; ++i)
            {
                var users = client.GetUsers(login.SecurityToken);
                Assert.AreEqual(1, users.Length);
                Console.WriteLine(i);
                Console.WriteLine(";");
            }
        }
    }
}
