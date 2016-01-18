using Cognite.Arb.Web.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Cognite.Arb.Web.Models.UserManagement
{
    public class UserViewModel
    {
        [Required(ErrorMessage = GlobalStrings.EmailIsRequired)]
        [EmailAddress(ErrorMessage = GlobalStrings.EmailIsNotValid)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = GlobalStrings.FirstNameIsRequired)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = GlobalStrings.LastNameIsRequired)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = GlobalStrings.RoleIsRequired)]
        [Display(Name = "User role")]
        public string SelectedRole { get; set; }

        [Required(ErrorMessage = GlobalStrings.CaseIsRequired)]
        [Display(Name = "Assigned case")]
        public string SelectedCase { get; set; }

        public string FullName { get; set; }

        public string AssignedCases { get; set; }

        public Guid Id { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        public IEnumerable<SelectListItem> Cases { get; set; }

        public UserViewModel()
        {
            this.Id = Guid.NewGuid();
            this.Roles = RolesHelper.GetRolesSelectListItems();
            this.Cases = new List<SelectListItem>();
        }
    }
}