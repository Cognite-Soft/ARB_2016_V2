using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Business.Cases;
using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.Contract.Cases;
using Cognite.Arb.Server.WebApi.ExceptionHandling;
using Cognite.Arb.Server.WebApi.Security;
using ActivityFeed = Cognite.Arb.Server.Contract.ActivityFeed;
using Allegation = Cognite.Arb.Server.Contract.Cases.Allegation;
using AllegationCollection = Cognite.Arb.Server.Contract.Cases.AllegationCollection;
using AllegationCommentType = Cognite.Arb.Server.Contract.Cases.AllegationCommentType;
using AllegationsUpdate = Cognite.Arb.Server.Contract.Cases.AllegationsUpdate;
using ArchitectContact = Cognite.Arb.Server.Contract.Cases.ArchitectContact;
using Case = Cognite.Arb.Server.Contract.Cases.Case;
using CaseContacts = Cognite.Arb.Server.Contract.Cases.CaseContacts;
using CaseHeader = Cognite.Arb.Server.Contract.CaseHeader;
using CasePanelMembers = Cognite.Arb.Server.Contract.Cases.CasePanelMembers;
using CaseState = Cognite.Arb.Server.Contract.Cases.CaseState;
using CaseStateKind = Cognite.Arb.Server.Contract.Cases.CaseStateKind;
using CaseUpdate = Cognite.Arb.Server.Contract.Cases.CaseUpdate;
using Contact = Cognite.Arb.Server.Contract.Cases.Contact;
using CreateCaseInfo = Cognite.Arb.Server.Contract.CreateCaseInfo;
using DateAndDetail = Cognite.Arb.Server.Contract.Cases.DateAndDetail;
using DateAndDetailUpdate = Cognite.Arb.Server.Contract.Cases.DateAndDetailUpdate;
using Document = Cognite.Arb.Server.Contract.Cases.Document;
using FinalDecisionComment = Cognite.Arb.Server.Contract.FinalDecisionComment;
using InitialCaseData = Cognite.Arb.Server.Contract.Cases.InitialCaseData;
using InitialCaseDataUpdate = Cognite.Arb.Server.Contract.Cases.InitialCaseDataUpdate;
using MyAllegationComment = Cognite.Arb.Server.Contract.Cases.MyAllegationComment;
using NewAllegation = Cognite.Arb.Server.Contract.Cases.NewAllegation;
using NewDateAndDetail = Cognite.Arb.Server.Contract.Cases.NewDateAndDetail;
using NewDocument = Cognite.Arb.Server.Contract.Cases.NewDocument;
using NewPartiesComment = Cognite.Arb.Server.Contract.Cases.NewPartiesComment;
using PanelMembersUpdate = Cognite.Arb.Server.Contract.Cases.PanelMembersUpdate;
using PartiesComment = Cognite.Arb.Server.Contract.Cases.PartiesComment;
using Post = Cognite.Arb.Server.Contract.Post;
using Question = Cognite.Arb.Server.Contract.Cases.Question;
using ReadonlyCaseData = Cognite.Arb.Server.Contract.Cases.ReadonlyCaseData;
using Reply = Cognite.Arb.Server.Contract.Reply;
using Role = Cognite.Arb.Server.Contract.Role;

namespace Cognite.Arb.Server.WebApi.Controllers
{
    [AuthorizationRequired]
    [Security.Authorize(Role.Admin, Role.CaseWorker, Role.PanelMember, Role.Inquirer, Role.Solicitor, Role.ThirdPartyReviewer)]
    public class CasesController : ApiController
    {
        static CasesController()
        {
            Mapper.CreateMap<CreateCaseInfo.ContactData, Business.CreateCaseInfo.ContactData>();
            Mapper.CreateMap<CreateCaseInfo, Business.CreateCaseInfo>();
            Mapper.CreateMap<Business.Database.CaseHeader, CaseHeader>();
            Mapper.CreateMap<Business.Cases.Case, Case>();
            Mapper.CreateMap<Business.Cases.ReadonlyCaseData, ReadonlyCaseData>();
            Mapper.CreateMap<Business.Cases.InitialCaseData, InitialCaseData>();
            Mapper.CreateMap<Business.Cases.CaseContacts, CaseContacts>();
            Mapper.CreateMap<Business.Cases.DateAndDetail, DateAndDetail>();
            Mapper.CreateMap<Business.Cases.AllegationCollection, AllegationCollection>();
            Mapper.CreateMap<Business.Cases.CaseState, CaseState>();
            Mapper.CreateMap<Business.Cases.CaseStateKind, CaseStateKind>();
            Mapper.CreateMap<Business.Cases.CasePanelMembers, CasePanelMembers>();
            Mapper.CreateMap<UserHeader, User>();
            Mapper.CreateMap<Business.Cases.Question, Question>();
            Mapper.CreateMap<Business.Cases.Contact, Contact>();
            Mapper.CreateMap<Business.Cases.ArchitectContact, ArchitectContact>();
            Mapper.CreateMap<Business.Cases.DateAndDetail, DateAndDetail>();
            Mapper.CreateMap<Business.Cases.Document, Document>();
            Mapper.CreateMap<Business.Cases.AllegationCollection, AllegationCollection>();
            Mapper.CreateMap<Business.Cases.Allegation, Allegation>();
            Mapper.CreateMap<Business.Cases.MyAllegationComment, MyAllegationComment>();
            Mapper.CreateMap<Business.Cases.AllegationCommentType, AllegationCommentType>();
            Mapper.CreateMap<Business.Cases.PartiesComment, PartiesComment>();

            Mapper.CreateMap<CaseUpdate, Business.Cases.CaseUpdate>();
            Mapper.CreateMap<InitialCaseDataUpdate, Business.Cases.InitialCaseDataUpdate>();
            Mapper.CreateMap<PanelMembersUpdate, Business.Cases.PanelMembersUpdate>();
            Mapper.CreateMap<Question, Business.Cases.Question>();
            Mapper.CreateMap<CaseContacts, Business.Cases.CaseContacts>();
            Mapper.CreateMap<Contact, Business.Cases.Contact>();
            Mapper.CreateMap<ArchitectContact, Business.Cases.ArchitectContact>();
            Mapper.CreateMap<DateAndDetailUpdate, Business.Cases.DateAndDetailUpdate>();
            Mapper.CreateMap<NewDateAndDetail, Business.Cases.NewDateAndDetail>();
            Mapper.CreateMap<AllegationsUpdate, Business.Cases.AllegationsUpdate>();
            Mapper.CreateMap<NewAllegation, Business.Cases.NewAllegation>();
            Mapper.CreateMap<NewDocument, Business.Cases.NewDocument>();

            Mapper.CreateMap<Business.ActivityFeed, ActivityFeed>();
            Mapper.CreateMap<Business.ActivityFeed.ActivityFeedSection, ActivityFeed.ActivityFeedSection>();
            Mapper.CreateMap<Business.ActivityFeed.ActivityFeedSectionHeader, ActivityFeed.ActivityFeedSectionHeader>();
            Mapper.CreateMap<Business.ActivityFeed.ActivityFeedItem, ActivityFeed.ActivityFeedItem>();
            Mapper.CreateMap<Business.ActivityFeed.ActivityFeedAction, ActivityFeed.ActivityFeedAction>();
            Mapper.CreateMap<Business.ActivityFeed.ActivityFeedSectionType, ActivityFeed.ActivityFeedSectionType>();
            Mapper.CreateMap<Business.ActivityFeed.ActivityFeedSectionStatus, ActivityFeed.ActivityFeedSectionStatus>();
        }

        [Route("api/cases", Name = "CreateCase", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.System)]
        [MapException(typeof (Facade.InvalidUserOrPasswordException), HttpStatusCode.Unauthorized)]
        public HttpResponseMessage CreateCase([FromBody] CreateCaseInfo @case)
        {
            var facade = Main.CreateFacade();
            var caseMapped = Mapper.Map<Business.CreateCaseInfo>(@case);
            facade.CreateCase(caseMapped, @case.CaseManagerEmail);
            var response = Request.CreateResponse<CreateCaseInfo>(HttpStatusCode.Created, null);
            response.Headers.Location = new Uri(Url.Link("GetCase", new {id = @case.Id}));
            return response;
        }

        [Route("api/cases", Name = "GetCaseHeaders", Order = 90)]
        [HttpGet]
        public IEnumerable<CaseHeader> GetCaseHeaders()
        {
            var facade = Main.CreateFacade();
            var cases = facade.GetCaseHeaders(CurrentUser.Get<UserHeader>());
            var casesMapped = cases.Select(Mapper.Map<CaseHeader>);
            return casesMapped;
        }

        [Route("api/cases/{id}", Name = "GetCase", Order = 100)]
        [HttpGet]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public Case GetCase(int id)
        {
            var facade = Main.CreateFacade();
            var @case = facade.GetCase(CurrentUser.Get<UserHeader>(), id);
            var caseMapped = Mapper.Map<Case>(@case);
            return caseMapped;
        }

        [Route("api/cases/{id}", Name = "UpdateCase", Order = 100)]
        [HttpPut]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void UpdateCase(int id, [FromBody] CaseUpdate caseUpdate)
        {
            var facade = Main.CreateFacade();
            var caseUpdateMapped = Mapper.Map<Business.Cases.CaseUpdate>(caseUpdate);
            facade.UpdateCase(CurrentUser.Get<UserHeader>(), id, caseUpdateMapped);
        }

        [Route("api/assignments/{id}", Name = "GetUserAssignments", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.Admin, Role.CaseWorker)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (UserDoesNotExistException), HttpStatusCode.NotFound)]
        public int[] GetUserAssignments(Guid id)
        {
            var facade = Main.CreateFacade();
            var cases = facade.GetUserAssignments(id);
            return cases;
        }

        [Route("api/assignments/{id}", Name = "UpdateUserAssignments", Order = 100)]
        [HttpPut]
        [Security.Authorize(Role.Admin, Role.CaseWorker)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (UserDoesNotExistException), HttpStatusCode.NotFound)]
        public void UpdateUserAssignments(Guid id, [FromBody] int[] caseIdCollection)
        {
            var facade = Main.CreateFacade();
            facade.UpdateUserAssignments(id, caseIdCollection);
        }

        [Route("api/allegations/{id}/comment", Name = "CommentAllegation", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.PanelMember)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void CommentAllegation(Guid id, [FromBody] AllegationComment comment)
        {
            var facade = Main.CreateFacade();
            facade.CommentAllegation(CurrentUser.Get<UserHeader>(), id, comment.Text,
                (Business.Cases.AllegationCommentType) comment.Type, comment.AdditionalText);
        }

        [Route("api/cases/{id}/start", Name = "StartProcessing", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.CaseWorker)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void StartProcessing(int id, [FromBody] AllegationComment comment)
        {
            var facade = Main.CreateFacade();
            facade.StartCaseProcessing(CurrentUser.Get<UserHeader>(), id);
        }

        [Route("api/cases/{id}/close", Name = "CloseCase", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.CaseWorker)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void CloseCase(int id)
        {
            var facade = Main.CreateFacade();
            facade.CloseCase(id);
        }

        [Route("api/cases/{id}/reopen", Name = "ReopenCase", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.CaseWorker)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void ReopenCase(int id)
        {
            var facade = Main.CreateFacade();
            facade.ReopenCase(id);
        }

        [Route("api/cases/{id}/clone", Name = "CloneCase", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.CaseWorker)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void CloneCase(int id)
        {
            var facade = Main.CreateFacade();
            facade.CloneCase(id);
        }

        [Route("api/activityfeed/{id}", Name = "GetActivityFeed", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.Admin, Role.CaseWorker, Role.PanelMember, Role.Solicitor, Role.ThirdPartyReviewer, Role.Inquirer)
        ]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public ActivityFeed GetActivityFeed(int id)
        {
            var facade = Main.CreateFacade();
            var activityFeed = facade.GetActivityFeed(id);
            var activityFeedMapped = Mapper.Map<ActivityFeed>(activityFeed);
            return activityFeedMapped;
        }

        [Route("api/cases/{id}/preliminarydecisioncomment", Name = "AddPreliminaryDecisionComment", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.PanelMember)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void AddPreliminaryDecisionComment(int id, [FromBody] string text)
        {
            var facade = Main.CreateFacade();
            facade.AddPreliminaryDecisionComment(CurrentUser.Get<UserHeader>(), id, text);
        }

        [Route("api/cases/{id}/finaldecisioncomment", Name = "AddFinalDecisionComment", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.PanelMember)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void AddFinalDecisionComment(int id, [FromBody] FinalDecisionComment comment)
        {
            var facade = Main.CreateFacade();
            facade.AddFinalDecisionComment(CurrentUser.Get<UserHeader>(), id, (FinalDecisionCommentKind) comment.Type,
                comment.CommentForChange);
        }

        [Route("api/cases/{id}/comments", Name = "GetComments", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.CaseWorker, Role.PanelMember)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public ComplaintComments GetComments(int id)
        {
            var facade = Main.CreateFacade();
            return facade.GetComments(CurrentUser.Get<UserHeader>(), id);
        }

        [Route("api/cases/{id}/posts", Name = "GetPosts", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.CaseWorker, Role.PanelMember, Role.Inquirer, Role.Solicitor, Role.ThirdPartyReviewer)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public Post[] GetPosts(int id)
        {
            var facade = Main.CreateFacade();
            var posts = facade.GetPosts(id);
            var result = posts.Select(x => new Post
            {
                Id = x.Id,
                Text = x.Text,
                User = new User
                {
                    Id = x.User.Id,
                    Email = x.User.Email,
                    Role = (Role) x.User.Role,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                },
                Parent = x.Parent == null
                    ? null
                    : new Reply
                    {
                        Id = x.Parent.Id,
                        Text = x.Parent.Text,
                        User = new User
                        {
                            Id = x.Parent.User.Id,
                            Email = x.Parent.User.Email,
                            Role = (Role) x.Parent.User.Role,
                            FirstName = x.Parent.User.FirstName,
                            LastName = x.Parent.User.LastName,
                        }
                    },
            }).ToArray();
            return result;
        }

        [Route("api/cases/{id}/posts/create", Name = "CreatePost", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.CaseWorker, Role.PanelMember, Role.Inquirer, Role.Solicitor, Role.ThirdPartyReviewer)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void CreatePost(int id, [FromBody] CreatePostRequest request)
        {
            var facade = Main.CreateFacade();
            facade.CreatePost(request.PostId, id, CurrentUser.Get<UserHeader>().Id, request.Text);
        }

        [Route("api/cases/{id}/posts/reply", Name = "CreateReply", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.CaseWorker, Role.PanelMember, Role.Inquirer, Role.Solicitor, Role.ThirdPartyReviewer)]
        [MapException(typeof (ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void CreateReply(int id, [FromBody] CreateReplyRequest request)
        {
            var facade = Main.CreateFacade();
            facade.CreateReply(request.ReplyId, id, request.ReplyToPostId, CurrentUser.Get<UserHeader>().Id, request.Text);
        }

        [Route("api/cases/{id}/partiescomment", Name = "AddPartiesComment", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.CaseWorker)]
        public void AddPartiesComment(int id, [FromBody] NewPartiesComment comment)
        {
            var facade = Main.CreateFacade();
            facade.AddPartiesComment(CurrentUser.Get<UserHeader>(),
                new Business.Cases.NewPartiesComment
                {
                    Id = comment.Id,
                    CaseId = id,
                    Text = comment.Text,
                    Documents = comment.Documents.Select(x => new Business.Cases.NewDocument
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Body = x.Body,
                    }).ToArray(),
                });
        }

        [Route("api/cases/{id}/preliminarydecision", Name = "AddPreliminaryDecisionDocument", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.CaseWorker)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void AddPreliminaryDecisionDocument(int id, [FromBody] NewDocument document)
        {
            var facade = Main.CreateFacade();
            facade.AddPreliminaryDecisionDocument(CurrentUser.Get<UserHeader>(), id, document);
        }

        [Route("api/cases/{id}/finaldecision", Name = "AddFinalDecisionDocument", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.CaseWorker, Role.PanelMember)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void AddFinalDecisionDocument(int id, [FromBody] NewDocument document)
        {
            var facade = Main.CreateFacade();
            facade.AddFinalDecisionDocument(CurrentUser.Get<UserHeader>(), id, document);
        }

        //newly added on sep 4th
        [Route("api/cases/{id}/preliminarydecisionupdate", Name = "UpdatePreliminaryDecisionDocument", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.CaseWorker, Role.PanelMember)]
        [MapException(typeof(CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void UpdatePreliminaryDecisionDocument(int id, [FromBody] NewDocument document)
        {
            var facade = Main.CreateFacade();
            facade.UpdatePreliminaryDecisionDocument(CurrentUser.Get<UserHeader>(), id, document);            
        }

        [Route("api/cases/{id}/finaldecisionupdate", Name = "UpdateFinalDecisionDocument", Order = 100)]
        [HttpPost]
        [Security.Authorize(Role.CaseWorker, Role.PanelMember)]
        [MapException(typeof (CaseDoesNotExistException), HttpStatusCode.NotFound)]
        public void UpdateFinalDecisionDocument(int id, [FromBody] NewDocument document)
        {
            var facade = Main.CreateFacade();
            facade.UpdateFinalDecisionDocument(CurrentUser.Get<UserHeader>(), id, document);
        }

        [Route("api/cases/{id}/alldocuments", Name = "GetAllDocuments", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.CaseWorker, Role.PanelMember, Role.Inquirer, Role.Solicitor, Role.ThirdPartyReviewer)]
        public Document[] GetAllDocuments(int id, [FromBody] NewDocument document)
        {
            var facade = Main.CreateFacade();
            var documents = facade.GetAllDocuments(id);
            var result = documents.Select(x => new Document
            {
                Id = x.Id,
                Name = x.Name,
            }).ToArray();
            return result;
        }

        [Route("api/scheduler/run", Name = "RunScheduler", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.System)]
        public string RunScheduler()
        {
            var facade = Main.CreateFacade();
            facade.NotifyCaseWorkersAboutClosingCases();
            return "Success";
        }

        //prabhakar
        [Route("api/cases/{id}/caseworkerdetailswdetails", Name = "GetCWDetails", Order = 100)]
        [HttpGet]
        [Security.Authorize(Role.CaseWorker, Role.PanelMember)]
        [MapException(typeof(ForbiddenException), HttpStatusCode.Forbidden)]
        [MapException(typeof(CaseDoesNotExistException), HttpStatusCode.NotFound)]
       // public HttpResponseMessage getCWDetails(int caseid)
        public HttpResponseMessage GetCWDetails(int id)
        {
            var facade = Main.CreateFacade();
            List<UserHeader> userHeader = facade.GetCaseWorkerDetails(id);
            var response = Request.CreateResponse<List<UserHeader>>(HttpStatusCode.OK, userHeader);
            return response;
        }
    }
}