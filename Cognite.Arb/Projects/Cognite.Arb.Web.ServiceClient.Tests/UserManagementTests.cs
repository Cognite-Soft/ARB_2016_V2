using System;
using System.Linq;
using Cognite.Arb.Server.Contract;
using NUnit.Framework;

namespace Cognite.Arb.Web.ServiceClient.Tests
{
    [TestFixture]
    public class UserManagementTests
    {
        [SetUp]
        public void Setup()
        {
            Database.Clear();
            DefaultUser.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            Database.Clear();
        }

        [Test]
        public void CanCreateUpdateDeleteUser()
        {
            var login = DefaultUser.Login();
            var client = DefaultUser.CreateWebApiClient();

            var users0 = client.GetUsers(login.SecurityToken);
            Assert.AreEqual(1, users0.Length);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "akisialiou@scnsoft.com",
                FirstName = "Alex",
                LastName = "Kisialiou",
                Role = Role.Admin,
            };
            user = client.CreateUser(login.SecurityToken, user);

            var users1 = client.GetUsers(login.SecurityToken);
            Assert.AreEqual(2, users1.Length);
            var readUser = users1.FirstOrDefault(item => item.Id == user.Id);
            Assert.IsNotNull(readUser);
            Assert.AreEqual(user.Email, readUser.Email);
            Assert.AreEqual(user.FirstName, readUser.FirstName);
            Assert.AreEqual(user.LastName, readUser.LastName);
            Assert.AreEqual(user.Role, readUser.Role);

            user.LastName = "LastName";
            client.UpdateUser(login.SecurityToken, user);

            var users2 = client.GetUsers(login.SecurityToken);
            Assert.AreEqual(2, users2.Length);
            var readUser2 = users2.FirstOrDefault(item => item.Id == user.Id);
            Assert.IsNotNull(readUser2);
            Assert.AreEqual(user.Email, readUser2.Email);
            Assert.AreEqual(user.FirstName, readUser2.FirstName);
            Assert.AreEqual(user.LastName, readUser2.LastName);
            Assert.AreEqual(user.Role, readUser2.Role);

            client.DeleteUser(login.SecurityToken, user.Id);

            var users3 = client.GetUsers(login.SecurityToken);
            Assert.AreEqual(1, users3.Length);
        }

        [Test]
        public void CanGetUserBySecurityToken()
        {
            var client = DefaultUser.CreateWebApiClient();
            var login = DefaultUser.Login();
            var user = client.GetUserBySecurityToken(login.SecurityToken);
            Assert.AreEqual(DefaultUser.Email, user.Email);
        }

        public static AuthenticationResult Login(string email, string password, string phrase)
        {
            var client = DefaultUser.CreateWebApiClient();
            var question = client.StartLoginAndGetSecurePhraseQuestion(email, password);
            var authenticationResult =
                client.FinishLoginWithSecurePhraseAnswer(question.SecurityToken, new SecurePhraseAnswer
                {
                    FirstCharacter = phrase[question.FirstCharacterIndex],
                    SecondCharacter = phrase[question.SecondCharacterIndex]
                });
            return authenticationResult;
        }

    }
}
