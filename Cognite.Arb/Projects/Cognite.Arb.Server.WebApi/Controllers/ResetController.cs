using System;
using System.Net;
using System.Web.Http;
using AutoMapper;
using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.WebApi.ExceptionHandling;

namespace Cognite.Arb.Server.WebApi.Controllers
{
    public class ResetController : ApiController
    {
        static ResetController()
        {
            Mapper.CreateMap<ResetTokenInfo, ResetToken>();            
        }

        [Route("api/reset/password", Name = "InitiateResetPassword", Order = 90)]
        [HttpPost]
        [MapException(typeof(Facade.UserDoesNotExistException), HttpStatusCode.NotFound)]
        public void InitiateResetPassword([FromBody] string email)
        {
            var facade = Main.CreateFacade();
            facade.InitiateResetPassword(email);
        }

        [Route("api/reset/phrase", Name = "InitiateResetSecurePhrase", Order = 90)]
        [HttpPost]
        [MapException(typeof(Facade.UserDoesNotExistException), HttpStatusCode.NotFound)]
        public void InitiateResetSecurePhrase([FromBody] string email)
        {
            // anybody can reset anybody's secure phrase
            var facade = Main.CreateFacade();
            facade.InitiateResetSecurePhrase(email);
        }

        [Route("api/reset/validate/{resetToken}", Name = "ValidateResetPasswordConfirmation", Order = 100)]
        [HttpGet]
        [MapException(typeof(Facade.InvalidResetTokenException), HttpStatusCode.NotFound)]
        [MapException(typeof(Facade.UserDoesNotExistException), HttpStatusCode.NotFound)]
        public ResetToken ValidateResetToken(string resetToken)
        {
            var facade = Main.CreateFacade();
            var tokenInfo = facade.ValidateResetToken(resetToken);
            var mappedTokenInfo = Mapper.Map<ResetToken>(tokenInfo);
            return mappedTokenInfo;
        }

        [Route("api/reset/password/{resetToken}", Name = "FinishResetPassword", Order = 100)]
        [HttpPut]
        [MapException(typeof(Facade.WeakPasswordException), HttpStatusCode.BadRequest)]
        [MapException(typeof(Facade.InvalidResetTokenException), HttpStatusCode.NotFound)]
        public void FinishResetPassword(string resetToken, [FromBody] string password)
        {
            var facade = Main.CreateFacade();
            facade.FinishResetPassword(resetToken, password);
        }

        [Route("api/reset/phrase/{resetToken}", Name = "FinishResetSecurePhrase", Order = 100)]
        [HttpPut]
        [MapException(typeof(Facade.WeakSecurePhraseException), HttpStatusCode.BadRequest)]
        [MapException(typeof(Facade.InvalidResetTokenException), HttpStatusCode.NotFound)]
        public void FinishResetSecurePhrase(string resetToken, [FromBody] string securePhrase)
        {
            var facade = Main.CreateFacade();
            facade.FinishResetSecurePhrase(resetToken, securePhrase);
        }

        [Route("api/reset/user/{resetToken}", Name = "FinishActivateUser", Order = 100)]
        [HttpPut]
        [MapException(typeof(Facade.WeakPasswordException), HttpStatusCode.BadRequest)]
        [MapException(typeof(Facade.WeakSecurePhraseException), HttpStatusCode.BadRequest)]
        [MapException(typeof(Facade.InvalidResetTokenException), HttpStatusCode.NotFound)]
        public void FinishActivateUser(string resetToken, [FromBody] Tuple<string, string> passwordAndSecurePhrase)
        {
            var facade = Main.CreateFacade();
            facade.FinishActivateUser(resetToken, passwordAndSecurePhrase.Item1, passwordAndSecurePhrase.Item2);
        }

        [Route("api/reset/checkcomplexity/{password}", Name = "CheckPasswordComplexity", Order = 100)]
        [HttpGet]
        [MapException(typeof(Facade.WeakSecurePhraseException), HttpStatusCode.BadRequest)]
        public void CheckPasswordComplexity(string password)
        {
            var facade = Main.CreateFacade();
            if (!facade.CheckPasswordComplexity(password))
                throw new Facade.WeakPasswordException();
        }
    }
}
