using System;
using System.Web.Http;
using AutoMapper;
using Cognite.Arb.Server.Business.Cases;
using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Server.Contract;
using Role = Cognite.Arb.Server.Business.Database.Role;

namespace Cognite.Arb.Server.WebApi.Controllers
{
    public class TestController : ApiController
    {
        static TestController()
        {
            Mapper.CreateMap<User, UserHeader>();
            Mapper.CreateMap<UserHeader, User>();
        }

        [Route("api/test/createdefaultuser", Name = "CreateDefaultUser", Order = 100)]
        [HttpGet]
        //[LocalCall]
        public User CreateDefaultUser(string password, string phrase)
        {
            var facade = Main.CreateFacade();
            var user = facade.CreateDefaultUser(password, phrase);
            var userMapped = Mapper.Map<User>(user);
            return userMapped;
        }

        [Route("api/test/createactiveuser", Name = "CreateActiveUser", Order = 100)]
        [HttpGet]
        public User CreateActiveUser(string id, string password, string phrase)
        {
            var facade = Main.CreateFacade();
            var role = Role.Admin;
            switch (id[0])
            {
                case 'c':
                    role = Role.CaseWorker;
                    break;
                case 'p':
                    role = Role.PanelMember;
                    break;
            }
            var user = facade.CreateActiveUser(id, password, phrase, role);
            var userMapped = Mapper.Map<User>(user);
            return userMapped;
        }

        [Route("api/test/setstartdate", Name = "SetCaseStartDate", Order = 100)]
        [HttpGet]
        public string SetCaseStartDate(int id, string date)
        {
            try
            {
                var facade = Main.CreateFacade();
                var dateTime = new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)),
                    int.Parse(date.Substring(6, 2)));
                facade.SetCaseStartDate(id, dateTime);
                return "Success";
            }
            catch (Exception)
            {
                return "Failure";
            }
        }
    }
}
