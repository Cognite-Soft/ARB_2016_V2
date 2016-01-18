using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.WebApi.ExceptionHandling;
using Cognite.Arb.Server.WebApi.Security;
using Role = Cognite.Arb.Server.Contract.Role;

namespace Cognite.Arb.Server.WebApi.Controllers
{
    [AuthorizationRequired]
    [Security.Authorize(Role.Admin, Role.CaseWorker)]
    public class UsersController : ApiController
    {
        static UsersController()
        {
            Mapper.CreateMap<User, UserHeader>();
            Mapper.CreateMap<UserHeader, User>();
        }

        // GET api/users
        [Route("api/users", Name = "GetAll", Order = 100)]
        [HttpGet]
        public IEnumerable<User> GetAll()
        {
            var facade = Main.CreateFacade();
            var users = facade.GetUsers();
            var mappedUsers = users.Select(Mapper.Map<User>).ToArray();
            return mappedUsers;
        }

        // GET api/users/id
        [Route("api/users/{id}", Name = "GetUser", Order = 100)]
        [HttpGet]
        [MapException(typeof(UserDoesNotExistException), HttpStatusCode.NotFound)]
        public User GetUser(Guid id)
        {
            var facade = Main.CreateFacade();
            var user = facade.GetUserById(id);
            var mappedUser = Mapper.Map<User>(user);
            return mappedUser;
        }

        // POST api/users
        [Route("api/users", Name = "CreateUser", Order = 100)]
        [HttpPost]
        [MapException(typeof (Facade.DuplicateUserException), HttpStatusCode.Conflict)]
        public HttpResponseMessage CreateUser(User user)
        {
            var userMapped = Mapper.Map<UserHeader>(user);
            var facade = Main.CreateFacade();
            var returnUser = facade.CreateUser(userMapped);
            var returnUserMapped = Mapper.Map<User>(returnUser);
            var response = Request.CreateResponse<User>(HttpStatusCode.Created, returnUserMapped);
            response.Headers.Location = new Uri(Url.Link("GetUser", new { id = returnUserMapped.Id }));
            return response;
        }

        // PUT api/users
        [Route("api/users/{id}", Name = "UpdateUser", Order = 100)]
        [HttpPut]
        [MapException(typeof(Facade.UserDoesNotExistException), HttpStatusCode.NotFound)]
        public void UpdateUser(Guid id, [FromBody] User user)
        {
            var userMapped = Mapper.Map<UserHeader>(user);
            Main.CreateFacade().UpdateUser(userMapped);
        }

        // DELETE api/users
        [Route("api/users/{id}", Name = "DeleteUser", Order = 100)]
        [HttpDelete]
        [MapException(typeof(Facade.UserDoesNotExistException), HttpStatusCode.NotFound)]
        public void DeleteUser(Guid id)
        {
            var facade = Main.CreateFacade();
            facade.DeleteUser(id);
        }
    }
}