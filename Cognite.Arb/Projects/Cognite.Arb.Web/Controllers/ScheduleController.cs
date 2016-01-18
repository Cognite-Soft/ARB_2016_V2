using Cognite.Arb.Server.Contract;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Web.Core.Authorization;
using Cognite.Arb.Web.Core.Mappers;
using Cognite.Arb.Web.Models;
using Cognite.Arb.Web.Models.Additional;
using Cognite.Arb.Web.Models.Schedule;
using Cognite.Arb.Web.ServiceClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cognite.Arb.Web.Controllers
{
    [Authorize(Roles = "CaseWorker")]
    public class ScheduleController : BaseController
    {
        // GET: /Schedule
        public ActionResult Index()
        {
            return TryGetSchedule();
        }

        private ActionResult TryGetSchedule()
        {
            var model = new ScheduleViewModel();

            try
            {
                CreateUsersList(this.SecurityToken);
                model = GetSchedule(this.SecurityToken);
            }
            catch (ForbiddenException)
            {
                AddModelStateError(GlobalStrings.Forbidden);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return View(model);
        }

        private void CreateUsersList(string token)
        {
            ViewBag.PanelMembersToComplete = GetUsersJson(token);
        }

        private string GetUsersJson(string token)
        {
            var users = this.Service.GetPanelMembers(token);
            return Mappers.MapUsersToJsonIdValueModel(users);
        }

        private ScheduleViewModel GetSchedule(string token)
        {
            var schedule = this.Service.GetSchedule(token);
            var orderedSchedule = schedule.OrderBy(item => item.LastUsed).ThenBy(item => item.Id);
            var model = GetScheduleModel(schedule);
            return model;
        }

        private ScheduleViewModel GetScheduleModel(IEnumerable<Schedule> schedules)
        {
            var model = new ScheduleViewModel();

            foreach (var schedule in schedules)
                model.ScheduleRows.Add(GetScheduleRow(schedule));

            return model;
        }

        private ScheduleRowViewModel GetScheduleRow(Schedule schedule)
        {
            var row = new ScheduleRowViewModel()
            {
                Index = schedule.Id,
                ScheduleRowItems = GetScheduleRowItems(schedule),
            };

            return row;
        }

        private List<ScheduleCellViewModel> GetScheduleRowItems(Schedule schedule)
        {
            var items = new List<ScheduleCellViewModel>();

            items.Add(GetScheduleRowItem(0, schedule.First));
            items.Add(GetScheduleRowItem(1, schedule.Second));
            items.Add(GetScheduleRowItem(2, schedule.Third));

            return items;
        }

        private ScheduleCellViewModel GetScheduleRowItem(int index, User user)
        {
            var item = new ScheduleCellViewModel()
            {
                Id = user == null ? Guid.Empty : user.Id,
                Index = index,
                Value = user == null ? String.Empty : user.FullName(),
            };

            return item;
        }

        // POST: /Schedule/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(int row, int col, Guid value)
        {
            var result = new JsonOperationModel(false, String.Empty);

            try
            {
                this.Service.UpdateScheduleCell(this.SecurityToken, row, col, value);
                var user = this.Service.GetUser(this.SecurityToken, value);
                var message = String.Format(GlobalStrings.SuccessfullyUpdatedSchedule, user.FullName());
                result.IsSucceeded = true;
                result.Result = message;
            }
            catch (ForbiddenException)
            {
                result.Result = GlobalStrings.Forbidden;
            }
            catch (ArgumentException)
            {
                result.Result = GlobalStrings.IncorrectData;
            }
            catch (UserDoesNotExistException)
            {
                result.Result = GlobalStrings.UserDoesNotExists;
            }
            catch (Exception)
            {
                result.Result = GlobalStrings.SomethingWentWrong;
            }
            
            return Json(result);
        }
    }
}
