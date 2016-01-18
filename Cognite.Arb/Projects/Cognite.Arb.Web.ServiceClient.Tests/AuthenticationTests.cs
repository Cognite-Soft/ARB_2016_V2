using System;
using Cognite.Arb.Server.Contract;
using NUnit.Framework;

namespace Cognite.Arb.Web.ServiceClient.Tests
{
    [TestFixture]
    public class AuthenticationTests
    {
        private const string IncorrectEmail = "incorrect";
        private const string IncorrectPassword = "incorrect";
        private const char IncorrectSecurePhraseSymbol = 'x';
        private const string UserFirstName = "Default";
        private const string UserLastName = "User";

        private WebApiClient _client;

        [SetUp]
        public void Setup()
        {
            Database.Clear();
            DefaultUser.Reset();
            _client = DefaultUser.CreateWebApiClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client = null;
            Database.Clear();
        }

        [Test, ExpectedException(typeof (InvalidUserOrPasswordException))]
        public void IncorrectUserIsNotAuthorized()
        {
            var client = DefaultUser.CreateWebApiClient();
            client.StartLoginAndGetSecurePhraseQuestion(IncorrectEmail, IncorrectPassword);
        }

        [Test, ExpectedException(typeof (InvalidUserOrPasswordException))]
        public void IncorrectPasswordIsNotAuthorized()
        {
            _client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, IncorrectPassword);
        }

        [Test]
        public void SecurePhraseQeustionDoesntChangeUntillAnswered()
        {
            var question1 = _client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, DefaultUser.Password);
            var question2 = _client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, DefaultUser.Password);

            Assert.AreNotEqual(question1.SecurityToken, question2.SecurityToken);
            Assert.AreEqual(question1.FirstCharacterIndex, question2.FirstCharacterIndex);
            Assert.AreEqual(question1.SecondCharacterIndex, question2.SecondCharacterIndex);
        }

        [Test, ExpectedException(typeof (NotAuthenticatedException))]
        public void IncorrectSecurePhraseAnswerTokenDoesNotAuthenticate()
        {
            _client.FinishLoginWithSecurePhraseAnswer("incorrect token", new SecurePhraseAnswer
            {
                FirstCharacter = IncorrectSecurePhraseSymbol,
                SecondCharacter = IncorrectSecurePhraseSymbol
            });
        }

        [Test, ExpectedException(typeof(InvalidSecurePhraseAnswer))]
        public void IncorrectSecurePhraseSymbol1DoesNotAuthenticate()
        {
            var question1 = _client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, DefaultUser.Password);
            _client.FinishLoginWithSecurePhraseAnswer(question1.SecurityToken, new SecurePhraseAnswer
            {
                FirstCharacter = IncorrectSecurePhraseSymbol,
                SecondCharacter = DefaultUser.Phrase[question1.SecondCharacterIndex]
            });
        }

        [Test, ExpectedException(typeof(InvalidSecurePhraseAnswer))]
        public void IncorrectSecurePhraseSymbol2DoesNotAuthenticate()
        {
            var question1 = _client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, DefaultUser.Password);
            _client.FinishLoginWithSecurePhraseAnswer(question1.SecurityToken, new SecurePhraseAnswer
            {
                FirstCharacter = DefaultUser.Phrase[question1.FirstCharacterIndex],
                SecondCharacter = IncorrectSecurePhraseSymbol,
            });
        }

        [Test, ExpectedException(typeof(InvalidSecurePhraseAnswer))]
        public void IncorrectSecurePhraseSymbolsDoesNotAuthenticate()
        {
            var question1 = _client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, DefaultUser.Password);
            _client.FinishLoginWithSecurePhraseAnswer(question1.SecurityToken, new SecurePhraseAnswer
            {
                FirstCharacter = IncorrectSecurePhraseSymbol,
                SecondCharacter = IncorrectSecurePhraseSymbol,
            });
        }

        [Test]
        public void SecurePhraseAnswerChangesNextSecurePhraseQeustion()
        {
            var question1 = _client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, DefaultUser.Password);
            var authenticationResult =
                _client.FinishLoginWithSecurePhraseAnswer(question1.SecurityToken, new SecurePhraseAnswer
                {
                    FirstCharacter = DefaultUser.Phrase[question1.FirstCharacterIndex],
                    SecondCharacter = DefaultUser.Phrase[question1.SecondCharacterIndex]
                });
            var question2 = _client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, DefaultUser.Password);

            Assert.AreEqual(Role.Admin, authenticationResult.Role);
            Assert.AreNotEqual(question1.SecurityToken, question2.SecurityToken);
            Assert.IsFalse(question1.FirstCharacterIndex == question2.FirstCharacterIndex &&
                           question1.SecondCharacterIndex == question2.SecondCharacterIndex);
        }

        [Test]
        public void UserCanBeRetrievedByTokenIfAuthenticated()
        {
            var authenticationResult = DefaultUser.Login();

            var user = _client.GetUserBySecurityToken(authenticationResult.SecurityToken);

            Assert.AreEqual(Guid.Empty, user.Id);
            Assert.AreEqual(DefaultUser.Email, user.Email);
            Assert.AreEqual(UserFirstName, user.FirstName);
            Assert.AreEqual(UserLastName, user.LastName);
            Assert.AreEqual(Role.Admin, user.Role);
        }

        [Test, ExpectedException(typeof(NotAuthenticatedException))]
        public void UserCanNotBeRetrievedByIncorrectToken()
        {
            _client.GetUserBySecurityToken("incorrecttoken");
        }

        [Test, ExpectedException(typeof(ForbiddenException))]
        public void UserNotLoggedInIsNotAuthenticated()
        {
            _client.GetUsers("IncorrectToken");
        }

        [Test, ExpectedException(typeof(ForbiddenException))]
        public void UserNotAnsweredSequrePhraseQuestionCannotBeAuthorized()
        {
            var question = _client.StartLoginAndGetSecurePhraseQuestion(DefaultUser.Email, DefaultUser.Password);
            _client.GetUsers(question.SecurityToken);
        }

        [Test]
        public void AuthenticatedUserCanBeAuthorized()
        {
            var login = DefaultUser.Login();

            var users = _client.GetUsers(login.SecurityToken);

            Assert.IsNotNull(users);
            Assert.AreEqual(1, users.Length);
        }
    }
}