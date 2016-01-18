using System.Net;
using System.Web.Http;
using AutoMapper;
using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.WebApi.ExceptionHandling;
using AuthenticationResult = Cognite.Arb.Server.Contract.AuthenticationResult;
using Login = Cognite.Arb.Server.Contract.Login;
using SecurePhraseAnswer = Cognite.Arb.Server.Contract.SecurePhraseAnswer;
using SecurePhraseQuestion = Cognite.Arb.Server.Contract.SecurePhraseQuestion;

namespace Cognite.Arb.Server.WebApi.Controllers
{
    public class TokensController : ApiController
    {
        static TokensController()
        {
            Mapper.CreateMap<Login, Business.Login>();
            Mapper.CreateMap<Business.SecurePhraseQuestion, SecurePhraseQuestion>();
            Mapper.CreateMap<SecurePhraseAnswer, Business.SecurePhraseAnswer>();
            Mapper.CreateMap<Business.AuthenticationResult, AuthenticationResult>();
            Mapper.CreateMap<UserHeader, User>();
        }

        /// <returns>
        /// HttpStatusCode.OK
        /// HttpStatusCode.Unauthorized
        /// </returns>
        [Route("api/tokens/", Name = "StartLoginAndGetSecurePhraseQuestion", Order = 100)]
        [HttpPost]
        [MapException(typeof (Facade.InvalidUserOrPasswordException), HttpStatusCode.Unauthorized)]
        public SecurePhraseQuestion StartLoginAndGetSecurePhraseQuestion(Login login)
        {
            var loginMapped = Mapper.Map<Business.Login>(login);
            var facade = Main.CreateFacade();
            var result = facade.StartLoginAndGetSecurePhraseQuestion(loginMapped);
            var resultMapped = Mapper.Map<SecurePhraseQuestion>(result);
            return resultMapped;
        }

        /// <returns>
        /// HttpStatusCode.OK
        /// HttpStatusCode.Unauthorized
        /// </returns>
        [Route("api/tokens/{token}", Name = "FinishLoginWithSecurePhraseAnswer", Order = 100)]
        [HttpPut]
        [MapException(typeof(Facade.NotAuthenticatedException), HttpStatusCode.Unauthorized)]
        [MapException(typeof(Facade.UserDoesNotExistException), HttpStatusCode.Unauthorized)]
        [MapException(typeof(Facade.InvalidSecurePhraseAnswerException), HttpStatusCode.BadRequest)]
        public AuthenticationResult FinishLoginWithSecurePhraseAnswer(string token, [FromBody] SecurePhraseAnswer answer)
        {
            var answerMapped = Mapper.Map<Business.SecurePhraseAnswer>(answer);
            var facade = Main.CreateFacade();
            var result = facade.FinishLoginWithSecurePhraseAnswer(token, answerMapped);
            var resultMapped = Mapper.Map<AuthenticationResult>(result);
            return resultMapped;
        }

        /// <returns>
        /// HttpStatusCode.OK
        /// HttpStatusCode.NotFound
        /// </returns>
        [Route("api/tokens/{token}", Name = "GetUserById", Order = 100)]
        [HttpGet]
        [MapException(typeof (Facade.SecurityTokenNotFoundException), HttpStatusCode.NotFound)]
        public User GetUser(string token)
        {
            var facade = Main.CreateFacade();
            var user = facade.GetUserByToken(token);
            var userMapped = Mapper.Map<User>(user);
            return userMapped;
        }

        /// <returns>
        /// HttpStatusCode.OK
        /// </returns>
        [Route("api/tokens/{token}", Name = "Logout", Order = 100)]
        [HttpDelete]
        public void Logout(string token)
        {
            Main.CreateFacade().Logout(token);
        }
    }
}