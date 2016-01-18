using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.WebApi.ExceptionHandling;
using Cognite.Arb.Server.WebApi.Security;

namespace Cognite.Arb.Server.WebApi.Controllers
{
    [AuthorizationRequired]
    [Security.Authorize(Role.Admin, Role.CaseWorker)]
    public class ScheduleController : ApiController
    {
        static ScheduleController()
        {
            Mapper.CreateMap<Business.Database.UserHeader, User>();
            Mapper.CreateMap<Business.Database.Schedule, Schedule>();
        }

        [Route("api/schedule", Name = "GetAllSchedules", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.Admin, Role.CaseWorker)]
        public IEnumerable<Schedule> GetAllSchedules()
        {
            var facade = Main.CreateFacade();
            var schedules = facade.GetSchedules();
            var schedulesMapped = schedules.Select(Mapper.Map<Schedule>);
            return schedulesMapped;
        }

        [Route("api/schedule/{row}/{column}", Name = "UpdateSchedule", Order = 100)]
        [HttpPut]
        [Security.Authorize(Role.Admin, Role.CaseWorker)]
        [MapException(typeof(ArgumentException), HttpStatusCode.NotFound)]
        [MapException(typeof(UserDoesNotExistException), HttpStatusCode.BadRequest)]
        public void UpdateSchedule(int row, int column, [FromBody] Guid userId)
        {
            var facade = Main.CreateFacade();
            facade.UpdateSchedule(row, column, userId);
        }
    }
}
