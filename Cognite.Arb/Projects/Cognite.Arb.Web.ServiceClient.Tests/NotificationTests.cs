using System;
using Cognite.Arb.Server.Contract;
using NUnit.Framework;
using Role = Cognite.Arb.Server.Business.Database.Role;

namespace Cognite.Arb.Web.ServiceClient.Tests
{
    [TestFixture]
    public class NotificationTests
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
        public void GeneralTest()
        {
            var fromUser = Database.CreateActiveUser("from@mail.com", Role.CaseWorker);
            var toUser = Database.CreateActiveUser("to@mail.com", Role.CaseWorker);
            var fromLogin = UserManagementTests.Login(fromUser.Email, Database.Password, Database.Phrase);
            var toLogin = UserManagementTests.Login(toUser.Email, Database.Password, Database.Phrase);

            var client = DefaultUser.CreateWebApiClient();
            var createNotification = new CreateNotification
            {
                ToUserId = toUser.Id,
                Message = "test message",
                Delivery = CreateNotification.DeliveryType.Both,
            };
            client.SendNotification(fromLogin.SecurityToken, createNotification);
            var fromNotifications = client.GetNotifications(fromLogin.SecurityToken);
            var toNotifications = client.GetNotifications(toLogin.SecurityToken);
            client.DismissNotification(toLogin.SecurityToken, toNotifications[0].Id);
            var toNotifications2 = client.GetNotifications(toLogin.SecurityToken);

            Assert.AreEqual(0, fromNotifications.Length);
            Assert.AreEqual(1, toNotifications.Length);
            Assert.AreEqual(0, toNotifications2.Length);
            Assert.AreEqual("test message", toNotifications[0].Message);
            Assert.Less((DateTime.Now - toNotifications[0].DateTime).TotalSeconds, 5);
        }
    }
}
