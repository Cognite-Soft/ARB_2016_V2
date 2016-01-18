using Cognite.Arb.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cognite.Arb.Web.Core.Authorization;
using Cognite.Arb.Web.Core;
using Cognite.Arb.Server.Contract;
using Newtonsoft.Json;
using Cognite.Arb.Web.Models.Additional;
using System.Text;
using Cognite.Arb.Web.Models.UserManagement;
using Cognite.Arb.Web.Core.Mappers;

namespace Cognite.Arb.Web.Controllers
{
    public partial class UserManagementController : BaseController
    {
        // GET: UserManagement
        public ActionResult Index()
        {
            var model = CreateBaseUserManagementVewModel();
            return View(model);
        }
        
        private UserManagementViewModel CreateBaseUserManagementVewModel()
        {
            var model = new UserManagementViewModel();

            try
            {
                TryAddUsers(this.SecurityToken, model);
                TryAddCases(this.SecurityToken, model.NewUser);
                CheckErrors();
            }
            catch (ForbiddenException)
            {
                AddModelStateError(GlobalStrings.Forbidden);
            }
            catch (Exception)
            {
                AddModelStateError(GlobalStrings.SomethingWentWrong);
            }

            return model;
        }
        
        private void TryAddUsers(string token, UserManagementViewModel model)
        {
            model.UserList = CreateUserManagementListModel(token);
            GetUsersAssignments(model.UserList, token);
        }
        
        private List<UserViewModel> CreateUserManagementListModel(string token)
        {
            var users = this.Service.GetUsers(token);
            return Mappers.MapUsersToUserViewModel(users);
        }

        private void GetUsersAssignments(List<UserViewModel> users, string token)
        {
            foreach (var user in users)
                user.AssignedCases = GetAssignedCases(user.Id, token);
        }
        
        private string GetAssignedCases(Guid userId, string token)
        {
            var cases = this.Service.GetAssignedCases(token, userId);
            var sb = new StringBuilder();

            for (int i = 0; i < cases.Length; i++)
                sb.Append(cases[i] + ",");

            return String.Format("[{0}]", sb.ToString().TrimEnd(','));
        }

        private void TryAddCases(string token, UserViewModel model)
        {
            var cases = GetCases(token);
            SetCasesJson(cases);
            model.Cases = GetCasesForNewUser(cases);
        }

        private void SetCasesJson(IEnumerable<IntStringModel> cases)
        {
            ViewBag.CasesToComplete = GetCasesJson(cases);
        }

        private string GetCasesJson(IEnumerable<IntStringModel> cases)
        {
            return JsonConvert.SerializeObject(cases);
        }

        private IEnumerable<IntStringModel> GetCases(string token)
        {
            var cases = this.Service.GetCases(token);
            return Mappers.MapCasesToIntStringModel(cases).OrderBy(i => i.Value);
        }

        private IEnumerable<SelectListItem> GetCasesForNewUser(IEnumerable<IntStringModel> cases)
        {
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem() { Value = "-1", Text = "None" });
            result.AddRange(from cs in cases
                            select new SelectListItem()
                            {
                                Value = cs.Id.ToString(),
                                Text = cs.Value
                            });
            return result;
        }

        private void CheckErrors()
        {
            var errorMessage = TempData[ArbConstants.AssignCaseErrorTempDataKey];
            if (errorMessage != null)
                AddModelStateError(errorMessage.ToString());
        }
    }
}
