using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Server.Business.Mailing;
using Moq;
using NUnit.Framework;

namespace Cognite.Server.Business.Tests
{
    [TestFixture]
    public class FacadeTests
    {
        private Facade _facade;
        private Mock<IDatabase> _databaseMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IPasswordManager> _passwordManagerMock;
        private Mock<ISecurityTokenService> _securityTokenServiceMock;
        private Mock<IMailNotifier> _mailNotifierMock;
        private IDatabase _database;
        private IConfiguration _configuration;
        private IPasswordManager _passwordManager;
        private ISecurityTokenService _securityTokenService;
        
        [SetUp]
        public void SetUp()
        {
            //_facade = new Facade(database, configuration, passwordManager, securityTokenService, mailNotifier);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Test()
        {

        }
    }
}
