using System;
using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Business.Mailing;
using Cognite.Arb.Server.Resource.Database;
using Cognite.Arb.Server.Resource.MailSender;
using Cognite.Arb.WebApi.Resource.Documents;

namespace Cognite.Arb.Server.WebApi
{
    public class Main
    {
        public static Facade CreateFacade()
        {
            var database = new Database();
            var configuration = new Configuration();
            var passwordManager = new PasswordManager(configuration);
            var securityTokenService = new Arb.WebApi.Resource.Sts.SecurityTokenService();
            var mailSender = new SmtpMailSender();
            var mailConfiguration = new MailConfiguration();
            var mailNotifier = new MailNotifier(mailSender, mailConfiguration);
            var documentStore = new SharePointDocumentStore();
            var facade = new Facade(database, configuration, passwordManager, securityTokenService, mailNotifier, documentStore);
            return facade;
        }

        private class Configuration : IConfiguration
        {
            public Configuration()
            {
                ResetPasswordTokenLifespan = TimeSpan.FromHours(36);
                ResetSecurePhraseTokenLifespan = TimeSpan.FromHours(36);
                ResetUserTokenLifespan = TimeSpan.FromHours(36);
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