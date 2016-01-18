using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.Contract.Cases;

namespace Cognite.Arb.Web.ServiceClient
{
    public class WebApiClient : IServiceClient
    {
        public const string DefaultUserMail = "admin@arb.org.uk";

        private const string SecurityTokenHeaderName = "SecurityToken";

        private readonly string _address;

        public WebApiClient(string address)
        {
            _address = address;
        }

        public User ResetDefaultUser(string password, string securePhrase)
        {
            return HttpClientDecorator.Get<User>(_address, null,
                string.Format("test/createdefaultuser?password={0}&phrase={1}", password, securePhrase),
                new ExceptionMapping(HttpStatusCode.Forbidden, typeof (ForbiddenException)));

            using (var client = CreateClient())
            {
                string uri = string.Format("users/createdefaultuser?password={0}&phrase={1}", password, securePhrase);
                var response = client.GetAsync(uri).Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<User>().Result;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                throw new Exception();
            }
        }

        public SecurePhraseQuestion StartLoginAndGetSecurePhraseQuestion(string email, string password)
        {
            return HttpClientDecorator.Post<SecurePhraseQuestion, Login>(_address, null, "tokens/",
                new Login {Email = email, Password = password},
                new ExceptionMapping(HttpStatusCode.Unauthorized, typeof (InvalidUserOrPasswordException)));

            using (var client = CreateClient())
            {
                var content = CreateHttpContent(new Login {Email = email, Password = password});
                var response = client.PostAsync("tokens/", content).Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<SecurePhraseQuestion>().Result;
                if (response.StatusCode == HttpStatusCode.Unauthorized) throw new InvalidUserOrPasswordException();
                throw new Exception();
            }
        }

        public AuthenticationResult FinishLoginWithSecurePhraseAnswer(string token, SecurePhraseAnswer answer)
        {
            return HttpClientDecorator.Put<AuthenticationResult, SecurePhraseAnswer>(_address, token, "tokens/" + token, answer,
                new ExceptionMapping(HttpStatusCode.BadRequest, typeof (InvalidSecurePhraseAnswer)),
                new ExceptionMapping(HttpStatusCode.Unauthorized, typeof (NotAuthenticatedException)));

            using (var client = CreateClient())
            {
                var content = CreateHttpContent(answer);
                var uri = "tokens/" + token;
                var response = client.PutAsync(uri, content).Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<AuthenticationResult>().Result;
                if (response.StatusCode == HttpStatusCode.BadRequest) throw new InvalidSecurePhraseAnswer();
                if (response.StatusCode == HttpStatusCode.Unauthorized) throw new NotAuthenticatedException();
                throw new Exception();
            }
        }

        public User GetUserBySecurityToken(string securityToken)
        {
            using (var client = CreateClient())
            {
                var uri = "tokens/" + securityToken;
                var response = client.GetAsync(uri).Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<User>().Result;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new NotAuthenticatedException();
                throw new Exception();
            }
        }

        public void Logout(string securityToken)
        {
            using (var client = CreateClient())
            {
                var uri = "tokens/" + securityToken;
                var response = client.DeleteAsync(uri).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new NotAuthenticatedException();
                throw new Exception();
            }
        }

        public User GetUser(string securityToken, Guid id)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("users/" + id).Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<User>().Result;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new UserDoesNotExistException();
                throw new Exception();
            }
        }

        public User[] GetUsers(string securityToken)
        {
            using (var client = CreateClient(securityToken))
            {
                var uri = "users";
                var response = client.GetAsync(uri).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                    return response.Content.ReadAsAsync<IEnumerable<User>>().Result.ToArray();
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                throw new Exception();
            }
        }

        public User CreateUser(string securityToken, User user)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(user);
                var response = client.PostAsync("users", content).Result;
                if (response.StatusCode == HttpStatusCode.Created) return response.Content.ReadAsAsync<User>().Result;
                if (response.StatusCode == HttpStatusCode.Conflict) throw new DuplicateUserException();
                throw new Exception();
            }
        }

        public void UpdateUser(string securityToken, User user)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(user);
                var response = client.PutAsync("users/" + user.Id, content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new UserDoesNotExistException();
                throw new Exception();
            }
        }

        public void DeleteUser(string securityToken, Guid id)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.DeleteAsync("users/" + id).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new UserDoesNotExistException();
                throw new Exception();
            }
        }

        public bool IsPasswordStrengthPassed(string password)
        {
            using (var client = CreateClient())
            {
                var response = client.GetAsync("reset/checkcomplexity/" + password).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return true;
                return false;
            }
        }

        public void InitiateResetPassword(string email)
        {
            using (var client = CreateClient())
            {
                var content = CreateHttpContent(email);
                var response = client.PostAsync("reset/password", content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new UserDoesNotExistException();
                throw new Exception();
            }
        }

        public void InitiateResetSecurePhrase(string securityToken, string email)
        {
            using (var client = CreateClient())
            {
                var content = CreateHttpContent(email);
                var response = client.PostAsync("reset/phrase", content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new UserDoesNotExistException();
                throw new Exception();
            }
        }

        public ResetToken ValidateResetToken(string resetToken)
        {
            using (var client = CreateClient())
            {
                var response = client.GetAsync("reset/validate/" + resetToken).Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<ResetToken>().Result;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new InvalidResetTokenException();
                throw new Exception();
            }
        }

        public void FinishResetPassword(string resetToken, string password)
        {
            using (var client = CreateClient())
            {
                var content = CreateHttpContent(password);
                var response = client.PutAsync("reset/password/" + resetToken, content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new InvalidResetTokenException();
                if (response.StatusCode == HttpStatusCode.BadRequest) throw new WeakPasswordException();
                throw new Exception();
            }
        }

        public void FinishResetSecurePhrase(string resetToken, string securePhrase)
        {
            using (var client = CreateClient())
            {
                var content = CreateHttpContent(securePhrase);
                var response = client.PutAsync("reset/phrase/" + resetToken, content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new InvalidResetTokenException();
                if (response.StatusCode == HttpStatusCode.BadRequest) throw new WeakSecurePhraseException();
                throw new Exception();
            }
        }

        public void FinishActivateUser(string resetToken, string password, string securePhrase)
        {
            using (var client = CreateClient())
            {
                var content = CreateHttpContent(new Tuple<string, string>(password, securePhrase));
                var response = client.PutAsync("reset/user/" + resetToken, content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.NotFound) throw new InvalidResetTokenException();
                if (response.StatusCode == HttpStatusCode.BadRequest) throw new WeakSecurePhraseException();
                throw new Exception();
            }
        }

        public CaseHeader[] GetCases(string securityToken)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("cases").Result;
                if (response.StatusCode == HttpStatusCode.OK)
                    return response.Content.ReadAsAsync<IEnumerable<CaseHeader>>().Result.ToArray();
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.BadRequest) throw new ArgumentException();
                throw new Exception();
            }
        }

        public CaseHeader[] GetRejectedCases(string securityToken)
        {
            var cases = GetCases(securityToken);
            var result = cases.Where(item => item.Status == CaseStatus.Rejected).ToArray();
            return result;
        }

        public CaseHeader[] GetClosedCases(string securityToken)
        {
            var cases = GetCases(securityToken);
            var result = cases.Where(item => item.Status == CaseStatus.Closed).ToArray();
            return result;
        }

        public CaseHeader[] GetActiveCases(string securityToken)
        {
            var cases = GetCases(securityToken);
            var result = cases.Where(item => item.Status == CaseStatus.Open).ToArray();
            return result;
        }

        public Case GetCase(string securityToken, int id)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("cases/" + id).Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<Case>().Result;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void UpdateCase(string securityToken, int caseId, CaseUpdate caseUpdate)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(caseUpdate);
                var response = client.PutAsync("cases/" + caseId, content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void StartCaseProcessing(string securityToken, int id)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("cases/" + id + "/start").Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void UpdateUserAssigments(string securityToken, Guid userId, params int[] caseIdCollection)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(caseIdCollection);
                var response = client.PutAsync("assignments/" + userId, content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new UserDoesNotExistException();
                ; // assignmentexception
                throw new Exception();
            }
        }

        public int[] GetAssignedCases(string securityToken, Guid userId)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("assignments/" + userId).Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<int[]>().Result;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new UserDoesNotExistException();
                throw new Exception();
            }
        }

        public void SendNotification(string securityToken, CreateNotification notification)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(notification);
                var response = client.PostAsync("notifications/", content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                throw new Exception();
            }
        }

        public void SendNotificationAll(string securityToken, CreateNotificationAll notification)
        {
            var targetCase = GetCase(securityToken, notification.CaseId);
            if (targetCase.InitialData.CasePanelMembers.PanelMember1 != null)
                CreateNotification(securityToken, targetCase.InitialData.CasePanelMembers.PanelMember1.Id, notification);
            if (targetCase.InitialData.CasePanelMembers.PanelMember2 != null)
                CreateNotification(securityToken, targetCase.InitialData.CasePanelMembers.PanelMember2.Id, notification);
            if (targetCase.InitialData.CasePanelMembers.PanelMember3 != null)
                CreateNotification(securityToken, targetCase.InitialData.CasePanelMembers.PanelMember3.Id, notification);
        }

        private void CreateNotification(string securityToken, Guid userId, CreateNotificationAll notification)
        {
            SendNotification(securityToken, new CreateNotification()
            {
                Delivery = notification.Delivery,
                Message = notification.Message,
                ToUserId = userId
            });
        }

        public Notification[] GetNotifications(string securityToken)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("notifications/").Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<Notification[]>().Result;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                throw new Exception();
            }
        }

        public void DismissNotification(string securityToken, Guid notificationId)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.DeleteAsync("notifications/" + notificationId).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new ArgumentException();
                throw new Exception();
            }
        }

        public void CommentAllegation(string securityToken, Guid allegationId, string comment, AllegationCommentType type,
            string additionalComment)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(new AllegationComment
                {
                    Text = comment,
                    Type = type,
                    AdditionalText = additionalComment,
                });
                var response = client.PostAsync("allegations/" + allegationId + "/comment", content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public ActivityFeed GetActivityFeed(string securityToken, int caseId)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("activityfeed/" + caseId).Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<ActivityFeed>().Result;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                throw new Exception();
            }
        }

        public Schedule[] GetSchedule(string securityToken)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("schedule/").Result;
                if (response.StatusCode == HttpStatusCode.OK)
                    return response.Content.ReadAsAsync<IEnumerable<Schedule>>().Result.ToArray();
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                throw new Exception();
            }
        }

        public void UpdateScheduleCell(string securityToken, int id, int colIndex, Guid userId)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(userId);
                var response = client.PutAsync("schedule/" + id + "/" + colIndex, content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new ArgumentException();
                if (response.StatusCode == HttpStatusCode.BadRequest) throw new UserDoesNotExistException();
                throw new Exception();
            }
        }

        public User[] GetPanelMembers(string securityToken)
        {
            var users = GetUsers(securityToken);
            var result = users.Where(item => item.Role == Role.PanelMember).ToArray();
            return result;
        }

        private HttpClient CreateClient(string securityToken = null)
        {
            var client = new HttpClient {BaseAddress = new Uri(_address)};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (securityToken != null)
                client.DefaultRequestHeaders.Add(SecurityTokenHeaderName, securityToken);
            return client;
        }

        private static ObjectContent CreateHttpContent<T>(T value)
        {
            var content = new ObjectContent(typeof (T), value, new JsonMediaTypeFormatter());
            return content;
        }

        public void CloseCase(string securityToken, int caseId)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("cases/" + caseId + "/close").Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void ReopenCase(string securityToken, int caseId)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("cases/" + caseId + "/reopen").Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void CloneCase(string securityToken, int caseId)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("cases/" + caseId + "/clone").Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public ComplaintComments GetComments(string securityToken, int caseId)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("cases/" + caseId + "/comments").Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<ComplaintComments>().Result;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void AddPreliminaryDecisionComment(string securityToken, int caseId, string text)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(text);
                var response = client.PostAsync("cases/" + caseId + "/preliminarydecisioncomment", content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void AddFinalDecisionComment(string securityToken, int caseId, FinalDecisionType finalDecisionType, string text)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(new FinalDecisionComment {Type = finalDecisionType, CommentForChange = text});
                var response = client.PostAsync("cases/" + caseId + "/finaldecisioncomment", content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public Post[] GetDiscussions(string securityToken, int caseId)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("cases/" + caseId + "/posts").Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<Post[]>().Result;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void CreatePost(string securityToken, int caseId, Guid postId, string text)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(new CreatePostRequest {PostId = postId, Text = text});
                var response = client.PostAsync("cases/" + caseId + "/posts/create", content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void ReplyOnPost(string securityToken, int caseId, Guid postId, Guid replyId, string text)
        {
            using (var client = CreateClient(securityToken))
            {
                var content = CreateHttpContent(new CreateReplyRequest {ReplyToPostId = postId, ReplyId = replyId, Text = text});
                var response = client.PostAsync("cases/" + caseId + "/posts/reply", content).Result;
                if (response.StatusCode == HttpStatusCode.NoContent) return;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
        }

        public void AddPartiesComment(string securityToken, int caseId, string text, NewDocument[] documents)
        {
            HttpClientDecorator.Post(_address, securityToken, "cases/" + caseId + "/partiescomment",
                new NewPartiesComment {Id = Guid.NewGuid(), Text = text, Documents = documents});
        }

        public NewDocument GetDocument(string securityToken, Guid documentId)
        {
            return HttpClientDecorator.Get<NewDocument>(_address, securityToken, "documents/" + documentId,
                new ExceptionMapping(HttpStatusCode.Forbidden, typeof (ForbiddenException)));
        }

        private class HttpClientDecorator : IDisposable
        {
            public static T Get<T>(string address, string securityToken, string uri, params ExceptionMapping[] mappings)
            {
                using (var client = new HttpClientDecorator(address, securityToken))
                {
                    return client.Get<T>(uri, mappings);
                }
            }

            public static T Post<T, P>(string address, string securityToken, string uri, P param,
                params ExceptionMapping[] mappings)
            {
                using (var client = new HttpClientDecorator(address, securityToken))
                {
                    return client.Post<T, P>(uri, param, mappings);
                }
            }

            public static void Post<P>(string address, string securityToken, string uri, P param,
                params ExceptionMapping[] mappings)
            {
                using (var client = new HttpClientDecorator(address, securityToken))
                {
                    client.Post<P>(uri, param, mappings);
                }
            }

            public static T Put<T, P>(string address, string securityToken, string uri, P param,
                params ExceptionMapping[] mappings)
            {
                using (var client = new HttpClientDecorator(address, securityToken))
                {
                    return client.Put<T, P>(uri, param, mappings);
                }
            }

            public static void Delete(string address, string uri, params ExceptionMapping[] mappings)
            {
                using (var client = new HttpClientDecorator(address))
                {
                    client.Delete(uri, mappings);
                }
            }

            private readonly HttpClient _client;
            private readonly Log _log = new Log();

            public HttpClientDecorator(string address, string securityToken = null)
            {
                _client = new HttpClient {BaseAddress = new Uri(address)};
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (securityToken != null)
                    _client.DefaultRequestHeaders.Add(SecurityTokenHeaderName, securityToken);
            }

            public T Get<T>(string uri, params ExceptionMapping[] mappings)
            {
                HttpResponseMessage response = null;
                try
                {
                    response = _client.GetAsync(uri).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = response.Content.ReadAsAsync<T>().Result;
                        return result;
                    }
                    foreach (var mapping in mappings)
                    {
                        if (mapping.Code == response.StatusCode)
                            throw (Exception) Activator.CreateInstance(mapping.Exception);
                    }
                    throw new Exception();
                }
                finally
                {
                    _log.Add(HttpMethod.Get, uri, null, response);
                }
            }

            public T Post<T, P>(string uri, P param, params ExceptionMapping[] mappings)
            {
                var content = CreateHttpContent(param);
                HttpResponseMessage response = null;
                try
                {
                    response = _client.PostAsync(uri, content).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = response.Content.ReadAsAsync<T>().Result;
                        return result;
                    }
                    foreach (var mapping in mappings)
                    {
                        if (mapping.Code == response.StatusCode)
                            throw (Exception) Activator.CreateInstance(mapping.Exception);
                    }
                    throw new Exception();
                }
                finally
                {
                    _log.Add(HttpMethod.Post, uri, content, response);
                }
            }

            private void Post<T>(string uri, T param, ExceptionMapping[] mappings)
            {
                var content = CreateHttpContent(param);
                HttpResponseMessage response = null;
                try
                {
                    response = _client.PostAsync(uri, content).Result;
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return;
                    }
                    foreach (var mapping in mappings)
                    {
                        if (mapping.Code == response.StatusCode)
                            throw (Exception) Activator.CreateInstance(mapping.Exception);
                    }
                    throw new Exception();
                }
                finally
                {
                    _log.Add(HttpMethod.Post, uri, content, response);
                }
            }

            public T Put<T, P>(string uri, P param, params ExceptionMapping[] mappings)
            {
                var content = CreateHttpContent(param);
                HttpResponseMessage response = null;
                try
                {
                    response = _client.PutAsync(uri, content).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = response.Content.ReadAsAsync<T>().Result;
                        return result;
                    }
                    foreach (var mapping in mappings)
                    {
                        if (mapping.Code == response.StatusCode)
                            throw (Exception) Activator.CreateInstance(mapping.Exception);
                    }
                    throw new Exception();
                }
                finally
                {
                    _log.Add(HttpMethod.Post, uri, content, response);
                }
            }

            public void Delete(string uri, params ExceptionMapping[] mappings)
            {
                HttpResponseMessage response = null;
                try
                {
                    response = _client.DeleteAsync(uri).Result;
                    if (response.StatusCode == HttpStatusCode.NoContent) return;
                    foreach (var mapping in mappings)
                    {
                        if (mapping.Code == response.StatusCode)
                            throw (Exception) Activator.CreateInstance(mapping.Exception);
                    }
                    throw new Exception();
                }
                finally
                {
                    _log.Add(HttpMethod.Post, uri, null, response);
                }
            }

            public void Dispose()
            {
                _client.Dispose();
            }

            private static ObjectContent CreateHttpContent<T>(T value)
            {
                var content = new ObjectContent(typeof (T), value, new JsonMediaTypeFormatter());
                return content;
            }

            private class Log
            {
                private readonly string _path;

                public Log()
                {
                    _path = ConfigurationManager.AppSettings["LogFilePath"];
                }

                public void Add(HttpMethod httpMethod, string uri, HttpContent requestContent, HttpResponseMessage response)
                {
                    if (_path == null) return;
                    var lines = new List<string>
                    {
                        DateTime.Now.ToString(),
                        String.Format("HTTP Method: {0}", httpMethod),
                        string.Format("URI: {0}", uri)
                    };
                    if (requestContent != null)
                        lines.Add(string.Format("Request: {0}", requestContent));
                    if (response != null)
                    {
                        lines.Add(string.Format("Response status code: {0}", response.StatusCode));
                        lines.Add(string.Format("Response: {0}", response));
                    }
                    lines.Add("----------------------------------------");
                    lines.Add(string.Empty);
                    File.AppendAllLines(_path, lines);
                }
            }
        }

        private class ExceptionMapping
        {
            public ExceptionMapping(HttpStatusCode code, Type exception)
            {
                Code = code;
                Exception = exception;
            }

            public HttpStatusCode Code { get; private set; }
            public Type Exception { get; private set; }
        }


        public void AddPreliminaryDecision(string securityToken, int caseId, NewDocument newDocument)
        {
            HttpClientDecorator.Post(_address, securityToken, "cases/" + caseId + "/preliminarydecision", newDocument,
                new ExceptionMapping(HttpStatusCode.Forbidden, typeof (ForbiddenException)),
                new ExceptionMapping(HttpStatusCode.NotFound, typeof (CaseDoesNotExistException)));
        }

        public void AddUpdatedPreliminaryDecision(string securityToken, int caseId, NewDocument newDocument)
        {
           // throw new NotImplementedException();

            //newly added on sep 4th
            HttpClientDecorator.Post(_address, securityToken, "cases/" + caseId + "/preliminarydecisionupdate", newDocument,
                new ExceptionMapping(HttpStatusCode.Forbidden, typeof(ForbiddenException)),
                new ExceptionMapping(HttpStatusCode.NotFound, typeof(CaseDoesNotExistException)));

        }

        public void AddFinalDecision(string securityToken, int caseId, NewDocument newDocument)
        {
            HttpClientDecorator.Post(_address, securityToken, "cases/" + caseId + "/finaldecision", newDocument,
                new ExceptionMapping(HttpStatusCode.Forbidden, typeof (ForbiddenException)),
                new ExceptionMapping(HttpStatusCode.NotFound, typeof (CaseDoesNotExistException)));
        }
        
        public void AddUpdatedFinalDecision(string securityToken, int caseId, NewDocument newDocument)
        {
            HttpClientDecorator.Post(_address, securityToken, "cases/" + caseId + "/finaldecisionupdate", newDocument,
                new ExceptionMapping(HttpStatusCode.Forbidden, typeof(ForbiddenException)),
                new ExceptionMapping(HttpStatusCode.NotFound, typeof(CaseDoesNotExistException)));
        }

        public Document[] GetCaseDocuments(string securityToken, int caseId)
        {
            return HttpClientDecorator.Get<Document[]>(_address, securityToken, "cases/" + caseId + "/alldocuments",
                new ExceptionMapping(HttpStatusCode.Forbidden, typeof (ForbiddenException)),
                new ExceptionMapping(HttpStatusCode.NotFound, typeof (CaseDoesNotExistException)));
        }

        public string NotifyCaseWorkersAboutClosingCases(string securityToken)
        {
            return HttpClientDecorator.Get<string>(_address, securityToken, "scheduler/run",
                new ExceptionMapping(HttpStatusCode.Forbidden, typeof (ForbiddenException)));
        }

        //prabhakar
        //public List<User> GetCaseWorkerforaCase(string securityToken, int caseid)
        public List<User> GetCaseWorkerforaCase(string securityToken, int caseid)
        {
            using (var client = CreateClient(securityToken))
            {
                var response = client.GetAsync("cases/" + caseid + "/caseworkerdetailswdetails").Result;
                //var response = client.GetAsync("cases/" + caseid + "/getCWDetails").Result;
                //var response = client.GetAsync("cases/Test").Result;
                //var response = client.GetAsync("cases/" + caseid + "/Test").Result;
                if (response.StatusCode == HttpStatusCode.OK) return response.Content.ReadAsAsync<List<User>>().Result;
                if (response.StatusCode == HttpStatusCode.Forbidden) throw new ForbiddenException();
                if (response.StatusCode == HttpStatusCode.NotFound) throw new CaseDoesNotExistException();
                throw new Exception();
            }
            
        }
    }
}