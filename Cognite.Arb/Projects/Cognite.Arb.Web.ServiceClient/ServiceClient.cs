using System;
using System.Collections.Generic;
using System.Linq;
using Cognite.Arb.Server.Contract;

namespace Cognite.Arb.Web.ServiceClient
{
    public class ServiceClient : IServiceClient
    {
        private static List<User> _users = new List<User>();
        private static Dictionary<string, User> _tokens = new Dictionary<string, User>();

        static ServiceClient()
        {
            _users = new List<User>
            {
                new User {Email = "admin@test.com", FirstName = "Admin", LastName = "Adninovich", Role = Role.Admin, Id=Guid.NewGuid()},
                new User {Email = "caseworker@test.com", FirstName = "Case", LastName = "Worker", Role = Role.CaseWorker, Id=Guid.NewGuid()},
                new User {Email = "panelmember@test.com", FirstName = "Panel", LastName = "Member", Role = Role.PanelMember, Id=Guid.NewGuid()},
                new User {Email = "inquirer@test.com", FirstName = "Inquirer", LastName = "Superior", Role = Role.Inquirer, Id=Guid.NewGuid()},
                new User {Email = "solicitor@test.com", FirstName = "Solicitor", LastName = "Superman", Role = Role.Solicitor, Id=Guid.NewGuid()},
                new User {Email = "reviewer@test.com", FirstName = "Reviewer", LastName = "Third-Party", Role = Role.ThirdPartyReviewer, Id=Guid.NewGuid()},
            };
        }

        public SecurePhraseQuestion StartLoginAndGetSecurePhraseQuestion(string email, string password)
        {
            var user = _users.FirstOrDefault(item => item.Email == email);
            if (user == null) throw new InvalidUserOrPasswordException();
            var token = Guid.NewGuid().ToString();
            _tokens.Add(token, user);
            return new SecurePhraseQuestion
            {
                SecurityToken = token,
                FirstCharacterIndex = 4,
                SecondCharacterIndex = 6,
            };
        }

        public AuthenticationResult FinishLoginWithSecurePhraseAnswer(string token, SecurePhraseAnswer answer)
        {
            if (!_tokens.ContainsKey(token)) throw new InvalidSecurePhraseAnswer();
            var user = _tokens[token];
            var newToken = Guid.NewGuid().ToString();
            _tokens.Remove(token);
            _tokens.Add(newToken, user);
            return new AuthenticationResult
            {
                Role = user.Role,
                SecurityToken = newToken,
            };
        }

        public User GetUserBySecurityToken(string securityToken)
        {
            if (!_tokens.ContainsKey(securityToken))
                return null;
            return _tokens[securityToken];
        }

        public void Logout(string securityToken)
        {
            _tokens.Remove(securityToken);
        }

        public void InitiateResetPassword(string email)
        {
        }

        public void InitiateResetSecurePhrase(string securityToken, string email)
        {
        }

        public ResetToken ValidateResetToken(string resetToken)
        {
            return new ResetToken()
            {
                Type = ResetToken.ResetType.Both,
                User = _users[0],
            };
        }

        public void FinishResetPassword(string resetToken, string password)
        {
        }

        public void FinishResetSecurePhrase(string resetToken, string securePhrase)
        {
        }

        public void FinishActivateUser(string resetToken, string password, string securePhrase)
        {
        }

        public User[] GetUsers(string securityToken)
        {
            return ServiceClient._users.ToArray();
        }

        public User CreateUser(string securityToken, User user)
        {
            _users.Add(user);
            return user;
        }

        public void UpdateUser(string securityToken, User user)
        {
            //throw new NotImplementedException();
            var userToupd = (from u in _users
                             where u.Email.Equals(user.Email)
                             select u).FirstOrDefault();

            if (userToupd != null)
            {
                userToupd.Email = user.Email;
                userToupd.FirstName = user.FirstName;
                userToupd.LastName = user.LastName;
                userToupd.Role = user.Role;
            }
            else
                _users.Add(userToupd);
        }

        public void DeleteUser(string securityToken, Guid id)
        {
        }


        public User GetUser(string securityToken, Guid id)
        {
            throw new NotImplementedException();
        }


        public object GetCasesToAssign(string securityToken)
        {
            throw new NotImplementedException();
        }

        public object GetSchedule(string securityToken)
        {
            throw new NotImplementedException();
        }

        public void UpdateScheduleCell(string securityToken, int rowIndex, int colIndex, Guid userId)
        {
            throw new NotImplementedException();
        }

        public object[] GetRejectedComplaints(string securityToken)
        {
            throw new NotImplementedException();
        }

        public object[] GetClosedComplaints(string securityToken)
        {
            throw new NotImplementedException();
        }

        public object[] GetActiveComplaints(string securityToken)
        {
            throw new NotImplementedException();
        }

        public object GetComplaint(string securityToken, int id)
        {
            throw new NotImplementedException();
        }
    }
}