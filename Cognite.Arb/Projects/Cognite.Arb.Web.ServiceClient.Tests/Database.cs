using System;
using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Server.Resource.Database;

namespace Cognite.Arb.Web.ServiceClient.Tests
{
    public static class Database
    {
        public const string Password = "0000";
        public const string Phrase = "0000";

        public static void Clear()
        {
            using (var context = new DatabaseContext())
            {
                context.Schedules.RemoveRange(context.Schedules);
                context.SaveChanges();
                context.Cases.RemoveRange(context.Cases);
                context.SaveChanges();
                context.ResetTokens.RemoveRange(context.ResetTokens);
                context.SaveChanges();
                context.Users.RemoveRange(context.Users);
                context.SaveChanges();
            }
        }

        public static UserEntity CreateActiveUser(string email, Role role)
        {
            using (var context = new DatabaseContext())
            {
                var passwordManager = new PasswordManager(new Configuration());
                var salt = passwordManager.GenerateSalt();
                var hashedPassword = passwordManager.HashPassword(Password, salt);
                var enctyptedSecurePhrase = passwordManager.EncryptSecurePhrase(Phrase);
                var user = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    FirstName = email,
                    LastName = email,
                    Role = role,
                    UserState = UserState.Activated,
                    PasswordSalt = salt,
                    HashedPassword = hashedPassword,
                    EncryptedSecurePhrase = enctyptedSecurePhrase,
                    FirstSecurePhraseQuestionCharacterIndex = 0,
                    SecondSecurePhraseQuestionCharacterIndex = 1,
                };
                context.Users.Add(user);
                context.SaveChanges();
                return user;
            }
        }

        private class Configuration : IConfiguration
        {
            public Configuration()
            {
                ResetPasswordTokenLifespan = TimeSpan.FromMinutes(5);
                ResetSecurePhraseTokenLifespan = TimeSpan.FromMinutes(5);
                ResetUserTokenLifespan = TimeSpan.FromMinutes(5);
                SecurePhraseEncryptionKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                SecurePhraseIv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            }

            public TimeSpan ResetPasswordTokenLifespan { get; private set; }
            public TimeSpan ResetSecurePhraseTokenLifespan { get; set; }
            public TimeSpan ResetUserTokenLifespan { get; set; }
            public byte[] SecurePhraseEncryptionKey { get; private set; }
            public byte[] SecurePhraseIv { get; private set; }
        }
    }
}
