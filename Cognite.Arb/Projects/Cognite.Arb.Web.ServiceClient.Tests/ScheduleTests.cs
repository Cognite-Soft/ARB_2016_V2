using NUnit.Framework;

namespace Cognite.Arb.Web.ServiceClient.Tests
{
    [TestFixture]
    public class ScheduleTests
    {
        [Test]
        public void GetOnFreshDbReturns10EmptySchedules()
        {
            Database.Clear();
            DefaultUser.Reset();
            var login = DefaultUser.Login();
            var client = DefaultUser.CreateWebApiClient();
            var schedules = client.GetSchedule(login.SecurityToken);
            Assert.AreEqual(10, schedules.Length);
            for (int i = 0; i < schedules.Length; ++i)
                Assert.AreEqual(i, schedules[i].Id);
        }

        [Test]
        public void CanUpdateSchedule()
        {
            Database.Clear();
            DefaultUser.Reset();
            var login = DefaultUser.Login();
            var client = DefaultUser.CreateWebApiClient();
            var id = client.GetUsers(login.SecurityToken)[0].Id;
            client.UpdateScheduleCell(login.SecurityToken, 3, 2, id);
            var schedules = client.GetSchedule(login.SecurityToken);
            Assert.AreEqual(10, schedules.Length);
            for (int i = 0; i < schedules.Length; ++i)
                Assert.AreEqual(i, schedules[i].Id);
            Assert.AreEqual(id, schedules[3].Third.Id);
        }
    }
}
