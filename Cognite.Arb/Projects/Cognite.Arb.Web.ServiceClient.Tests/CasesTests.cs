using System;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using Cognite.Arb.Integration.Business;
using Cognite.Arb.Integration.Resource.WebApi;
using Cognite.Arb.Server.Business.Cases;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.Contract.Cases;
using NUnit.Framework;
using AllegationCommentType = Cognite.Arb.Server.Contract.Cases.AllegationCommentType;
using AllegationsUpdate = Cognite.Arb.Server.Contract.Cases.AllegationsUpdate;
using CaseStateKind = Cognite.Arb.Server.Contract.Cases.CaseStateKind;
using CaseUpdate = Cognite.Arb.Server.Business.Cases.CaseUpdate;
using InitialCaseDataUpdate = Cognite.Arb.Server.Contract.Cases.InitialCaseDataUpdate;
using NewAllegation = Cognite.Arb.Server.Contract.Cases.NewAllegation;
using NewDocument = Cognite.Arb.Server.Contract.Cases.NewDocument;
using PanelMembersUpdate = Cognite.Arb.Server.Contract.Cases.PanelMembersUpdate;
using Question = Cognite.Arb.Server.Contract.Cases.Question;
using Role = Cognite.Arb.Server.Business.Database.Role;

namespace Cognite.Arb.Web.ServiceClient.Tests
{
    [TestFixture]
    public class CasesTests
    {
        [SetUp]
        public void Setup()
        {
            Database.Clear();
            DefaultUser.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            Database.Clear();
        }

        [Test]
        public void CanCreateAndGetCase()
        {
            var caseData = new Integrator.CaseData
            {
                Id = 10,
                CaseManagerEmail = DefaultUser.Email,
                ClaimantContact = new Integrator.CaseData.ContactData
                {
                    Name = "name",
                    EMail = "claimant@email.com",
                    Address = "address",
                    Phone = "+375297775218",
                },
                StartDate = DateTime.Now,
            };
            var destination = new Destination(ConfigurationManager.AppSettings["WebApiUrl"]);
            destination.CreateCase(caseData);

            var client = DefaultUser.CreateWebApiClient();

            var login = DefaultUser.Login();

            var readCase = client.GetCase(login.SecurityToken, caseData.Id);

            Assert.IsNotNull(readCase);

            Assert.AreEqual(Server.Contract.Cases.CaseStateKind.New, readCase.ReadonlyData.State.StateKind);
            Assert.AreEqual(caseData.Id, readCase.ReadonlyData.Id);
            Assert.AreEqual(caseData.ClaimantContact.EMail, readCase.Contacts.ClaimantContact.EMail);
            Assert.AreEqual(caseData.ClaimantContact.Address, readCase.Contacts.ClaimantContact.Address);
            Assert.AreEqual(caseData.ClaimantContact.Name, readCase.Contacts.ClaimantContact.Name);
            Assert.AreEqual(caseData.ClaimantContact.Phone, readCase.Contacts.ClaimantContact.Phone);
            Assert.IsNull(readCase.Contacts.ArchitectContact.Name);
            Assert.IsNull(readCase.Contacts.ArchitectContact.Address);
            Assert.IsNull(readCase.Contacts.ArchitectContact.EMail);
            Assert.IsNull(readCase.Contacts.ArchitectContact.Phone);
            Assert.IsNull(readCase.Contacts.ArchitectContact.RegistrationNumber);
            Assert.IsNull(readCase.InitialData.Background);
            Assert.IsNull(readCase.Contacts.Relationship);
            Assert.IsNull(readCase.InitialData.IdealOutcome);
            Assert.IsNull(readCase.Contacts.ContactAgreement);
            Assert.AreEqual(0, readCase.Allegations.Items.Length);
            Assert.AreEqual(0, readCase.DatesAndDetails.Length);
        }

        [Test]
        public void CanGetAllCases()
        {
            var caseData = CreateCase(1);

            var login = DefaultUser.Login();
            var client = DefaultUser.CreateWebApiClient();
            var cases = client.GetCases(login.SecurityToken);

            Assert.AreEqual(1, cases.Length);
            Assert.AreEqual(caseData.Id, cases[0].Id);
            Assert.AreEqual(caseData.ClaimantContact.Name, cases[0].Complainant);
            Assert.IsNull(cases[0].Architect);
            Assert.IsNull(cases[0].RegistrationNumber);
        }

        [Test]
        public void CanOnlyGetAssignedCasesIfLimitedRole()
        {
            var caseData1 = CreateCase(1);
            var caseData2 = CreateCase(2);
            var caseData3 = CreateCase(3);
            var user1 = Database.CreateActiveUser("user1@mail.com", Server.Business.Database.Role.PanelMember);
            var user2 = Database.CreateActiveUser("user2@mail.com", Server.Business.Database.Role.PanelMember);
            var user3 = Database.CreateActiveUser("user3@mail.com", Server.Business.Database.Role.PanelMember);
            var login = DefaultUser.Login();
            var client = DefaultUser.CreateWebApiClient();
            client.UpdateUserAssigments(login.SecurityToken, user1.Id, caseData1.Id, caseData2.Id);
            client.UpdateUserAssigments(login.SecurityToken, user2.Id, caseData2.Id, caseData3.Id);

            var login1 = UserManagementTests.Login(user1.Email, Database.Password, Database.Phrase);
            var login2 = UserManagementTests.Login(user2.Email, Database.Password, Database.Phrase);
            var login3 = UserManagementTests.Login(user3.Email, Database.Password, Database.Phrase);
            var cases1 = client.GetCases(login1.SecurityToken);
            var cases2 = client.GetCases(login2.SecurityToken);
            var cases3 = client.GetCases(login3.SecurityToken);

            Assert.AreEqual(2, cases1.Length);
            Assert.AreEqual(caseData1.Id, cases1[0].Id);
            Assert.AreEqual(caseData2.Id, cases1[1].Id);
            Assert.AreEqual(2, cases2.Length);
            Assert.AreEqual(caseData2.Id, cases2[0].Id);
            Assert.AreEqual(caseData3.Id, cases2[1].Id);
            Assert.AreEqual(0, cases3.Length);
        }

        [Test]
        public void CanGetAssignedUsers()
        {
            var caseData1 = CreateCase(1);
            var caseData2 = CreateCase(2);
            var caseData3 = CreateCase(3);
            var login = DefaultUser.Login();
            var client = DefaultUser.CreateWebApiClient();
            client.UpdateUserAssigments(login.SecurityToken, Guid.Empty, caseData1.Id, caseData2.Id);

            var cases1 = client.GetAssignedCases(login.SecurityToken, Guid.Empty);

            Assert.AreEqual(2, cases1.Length);
            Assert.AreEqual(caseData1.Id, cases1[0]);
            Assert.AreEqual(caseData2.Id, cases1[1]);
        }

        [Test]
        public void CanUpdateCase()
        {
            var caseData = CreateCase(1);

            var login = DefaultUser.Login();
            var client = DefaultUser.CreateWebApiClient();

            var pm1 = Database.CreateActiveUser("user1@mail.com", Role.PanelMember);
            var pm2 = Database.CreateActiveUser("user2@mail.com", Role.PanelMember);
            var pm3 = Database.CreateActiveUser("user3@mail.com", Role.PanelMember);
            var pm4 = Database.CreateActiveUser("user4@mail.com", Role.PanelMember);

            var caseUpdate = new Server.Contract.Cases.CaseUpdate
            {
                Initial = new Server.Contract.Cases.InitialCaseDataUpdate
                {
                    StartDate = DateTime.Now,
                    PanelMembers = new Server.Contract.Cases.PanelMembersUpdate
                    {
                        PanelMember1 = pm1.Id,
                        PanelMember2 = pm2.Id,
                        PanelMember3 = null,
                    },
                    IssueRaisedWithArchitect = new Server.Contract.Cases.Question
                    {
                        Answer = true,
                        Comments = "text bla bla bla",
                    },
                    SubjectOfLegalProceedings = new Server.Contract.Cases.Question(),
                },
                Contacts = new Server.Contract.Cases.CaseContacts
                {
                    ArchitectContact = new Server.Contract.Cases.ArchitectContact(),
                    ClaimantContact = new Server.Contract.Cases.Contact(),
                },
                AllegationsUpdate = new Server.Contract.Cases.AllegationsUpdate
                {
                    DeletedAllegations = new Guid[0],
                    NewAllegations = new Server.Contract.Cases.NewAllegation[0],
                },
                DatesAndDetailsUpdate = new Server.Contract.Cases.DateAndDetailUpdate
                {
                    DeletedDatesAndDetails = new Guid[0],
                    NewDatesAndDetails = new Server.Contract.Cases.NewDateAndDetail[0],
                }
            };
            client.UpdateCase(login.SecurityToken, caseData.Id, caseUpdate);

            var case1 = client.GetCase(login.SecurityToken, caseData.Id);

            Assert.AreEqual(Server.Contract.Cases.CaseStateKind.New, case1.ReadonlyData.State.StateKind);
            Assert.AreEqual(caseUpdate.Initial.StartDate, case1.InitialData.StartDate);
            var membersExpected = new[]
            {caseUpdate.Initial.PanelMembers.PanelMember1.Value, caseUpdate.Initial.PanelMembers.PanelMember2.Value}.OrderBy(
                item => item).ToArray();
            var membersActual =
                new[] {case1.InitialData.CasePanelMembers.PanelMember1.Id, case1.InitialData.CasePanelMembers.PanelMember2.Id}
                    .OrderBy(item => item).ToArray();
            Assert.AreEqual(membersExpected[0], membersActual[0]);
            Assert.AreEqual(membersExpected[1], membersActual[1]);

            Assert.IsNull(caseUpdate.Initial.PanelMembers.PanelMember3);

            caseUpdate.Initial.PanelMembers.PanelMember1 = pm3.Id;
            caseUpdate.Initial.PanelMembers.PanelMember3 = pm4.Id;

            client.UpdateCase(login.SecurityToken, caseData.Id, caseUpdate);

            var case2 = client.GetCase(login.SecurityToken, caseData.Id);
            Assert.AreEqual(Server.Contract.Cases.CaseStateKind.New, case2.ReadonlyData.State.StateKind);

            membersExpected = new[]
            {
                caseUpdate.Initial.PanelMembers.PanelMember1.Value,
                caseUpdate.Initial.PanelMembers.PanelMember2.Value, caseUpdate.Initial.PanelMembers.PanelMember3.Value
            }.OrderBy(
                item => item).ToArray();
            membersActual =
                new[]
                {
                    case2.InitialData.CasePanelMembers.PanelMember1.Id,
                    case2.InitialData.CasePanelMembers.PanelMember2.Id
                    , case2.InitialData.CasePanelMembers.PanelMember3.Id,
                }
                    .OrderBy(item => item).ToArray();
            Assert.AreEqual(membersExpected[0], membersActual[0]);
            Assert.AreEqual(membersExpected[1], membersActual[1]);
            Assert.AreEqual(membersExpected[2], membersActual[2]);
        }

        [Test]
        public void PanelMemberCanCommentAllegation()
        {
            var caseWorker = Database.CreateActiveUser("caseworker@mail.com", Role.CaseWorker);
            var panelMember1 = Database.CreateActiveUser("panelmember1@mail.com", Role.PanelMember);
            var panelMember2 = Database.CreateActiveUser("panelmember2@mail.com", Role.PanelMember);
            var panelMember3 = Database.CreateActiveUser("panelmember3@mail.com", Role.PanelMember);
            var caseWorkerLogin = UserManagementTests.Login(caseWorker.Email, Database.Password, Database.Phrase);
            var panelMemberLogin = UserManagementTests.Login(panelMember1.Email, Database.Password, Database.Phrase);
            var caseData = CreateCase(1);
            var client = DefaultUser.CreateWebApiClient();
            client.UpdateUserAssigments(caseWorkerLogin.SecurityToken, caseWorker.Id, caseData.Id);
            client.UpdateUserAssigments(caseWorkerLogin.SecurityToken, panelMember1.Id, caseData.Id);
            var caseUpdate = new Server.Contract.Cases.CaseUpdate
            {
                Initial = new InitialCaseDataUpdate
                {
                    Background = "a",
                    IdealOutcome = "a",
                    PanelMembers = new PanelMembersUpdate
                    {
                        PanelMember1 = panelMember1.Id,
                        PanelMember2 = panelMember2.Id,
                        PanelMember3 = panelMember3.Id,
                    },
                    IssueRaisedWithArchitect = new Question
                    {
                        Answer = false,
                        Comments = "a",
                    },
                    SubjectOfLegalProceedings = new Question
                    {
                        Answer = false,
                        Comments = "a",
                    },
                    StartDate = DateTime.Today.AddDays(-1),
                },
                AllegationsUpdate = new AllegationsUpdate
                {
                    NewAllegations = new[]
                    {
                        new NewAllegation
                        {
                            Id = Guid.NewGuid(),
                            Text = "Allegation",
                            Documents = new NewDocument[0],
                        },
                    },
                    DeletedAllegations = new Guid[0],
                }
            };
            client.UpdateCase(caseWorkerLogin.SecurityToken, caseData.Id, caseUpdate);
            client.StartCaseProcessing(caseWorkerLogin.SecurityToken, caseData.Id);

            client.CommentAllegation(panelMemberLogin.SecurityToken, caseUpdate.AllegationsUpdate.NewAllegations[0].Id, "comment",
                AllegationCommentType.Yes, null);

            var @case = client.GetCase(panelMemberLogin.SecurityToken, caseData.Id);
            Assert.AreEqual(AllegationCommentType.Yes, @case.Allegations.Items[0].MyComment.AllegationCommentType);
            Assert.AreEqual("comment", @case.Allegations.Items[0].MyComment.Text);
        }

        [Test]
        public void CanAddAndGetPartiesComments()
        {
            var caseData = new Integrator.CaseData
            {
                Id = 10,
                CaseManagerEmail = DefaultUser.Email,
                ClaimantContact = new Integrator.CaseData.ContactData
                {
                    Name = "name",
                    EMail = "claimant@email.com",
                    Address = "address",
                    Phone = "+375297775218",
                },
                StartDate = DateTime.Now,
            };
            var destination = new Destination(ConfigurationManager.AppSettings["WebApiUrl"]);
            destination.CreateCase(caseData);

            var caseWorker = Database.CreateActiveUser("caseworker@mail.com", Role.CaseWorker);
            var caseWorkerToken = UserManagementTests.Login(caseWorker.Email, Database.Password, Database.Phrase).SecurityToken;

            var client = DefaultUser.CreateWebApiClient();
            client.AddPartiesComment(caseWorkerToken, caseData.Id, "supertext", new NewDocument[]
            {
                new NewDocument
                {
                    Id = Guid.NewGuid(),
                    Name = "document1",
                    Body = new byte[] {1, 2, 3, 4, 5},
                },
                new NewDocument
                {
                    Id = Guid.NewGuid(),
                    Name = "document2",
                    Body = new byte[] {6, 7, 8, 9, 10},
                }
            });

            var login = DefaultUser.Login();
            var readCase = client.GetCase(login.SecurityToken, caseData.Id);
            Assert.AreEqual(1, readCase.PartiesComments.Length);
            Assert.AreEqual("supertext", readCase.PartiesComments[0].Text);
            Assert.AreEqual(2, readCase.PartiesComments[0].Documents.Length);
            var documentNames = readCase.PartiesComments[0].Documents.Select(x => x.Name).OrderBy(x => x).ToArray();
            Assert.AreEqual("document1", documentNames[0]);
            Assert.AreEqual("document2", documentNames[1]);
        }

        [Test, Explicit]
        public void CanAddPreliminaryDecisionDocument()
        {
            var caseData = new Integrator.CaseData
            {
                Id = 10,
                CaseManagerEmail = DefaultUser.Email,
                ClaimantContact = new Integrator.CaseData.ContactData
                {
                    Name = "name",
                    EMail = "claimant@email.com",
                    Address = "address",
                    Phone = "+375297775218",
                },
                StartDate = DateTime.Now,
            };
            var destination = new Destination(ConfigurationManager.AppSettings["WebApiUrl"]);
            destination.CreateCase(caseData);

            var caseWorker = Database.CreateActiveUser("caseworker@mail.com", Role.CaseWorker);
            var caseWorkerToken = UserManagementTests.Login(caseWorker.Email, Database.Password, Database.Phrase).SecurityToken;

            var client = DefaultUser.CreateWebApiClient();
            client.AddPreliminaryDecision(caseWorkerToken, caseData.Id,
                new NewDocument {Id = Guid.NewGuid(), Name = "document", Body = new byte[] {1, 2, 3}});
            var documents = client.GetCaseDocuments(caseWorkerToken, caseData.Id);
            var @case = client.GetCase(caseWorkerToken, caseData.Id);
            Assert.AreEqual(1, documents.Length);
            Assert.AreEqual("document", documents[0].Name);
            Assert.AreEqual("document", @case.PreliminaryDecisionDocument.Name);

            var doc = client.GetDocument(caseWorkerToken, documents[0].Id);
            Console.WriteLine(doc.Body.Length);
        }

        [Test]
        public void StatusChangeTest()
        {
            //var systemTime = new SYSTEMTIME();
            //Win32GetSystemTime(ref systemTime);

            var caseData = CreateCase(10);

            var panelMember1 = Database.CreateActiveUser("user1@mail.com", Role.PanelMember);
            var panelMember2 = Database.CreateActiveUser("user2@mail.com", Role.PanelMember);
            var panelMember3 = Database.CreateActiveUser("user3@mail.com", Role.PanelMember);
            var caseWorker = Database.CreateActiveUser("user4@mail.com", Role.CaseWorker);

            var caseWorkerToken = UserManagementTests.Login(caseWorker.Email, Database.Password, Database.Phrase).SecurityToken;
            var client = DefaultUser.CreateWebApiClient();
            client.UpdateUserAssigments(caseWorkerToken, caseWorker.Id, caseData.Id);
            var case1 = client.GetCase(caseWorkerToken, caseData.Id);
            client.UpdateCase(caseWorkerToken, caseData.Id, new Server.Contract.Cases.CaseUpdate
            {
                Contacts = case1.Contacts,
                Initial = new InitialCaseDataUpdate
                {
                    PanelMembers = new PanelMembersUpdate
                    {
                        PanelMember1 = panelMember1.Id,
                        PanelMember2 = panelMember2.Id,
                        PanelMember3 = panelMember3.Id,
                    },
                    StartDate = DateTime.Today,
                    Background = "aaa",
                    IdealOutcome = "bbb",
                    IssueRaisedWithArchitect = new Question { Answer = false,},
                    SubjectOfLegalProceedings = new Question { Answer = true, },
                },
            });
            client.StartCaseProcessing(caseWorkerToken, caseData.Id);
            var case2 = client.GetCase(caseWorkerToken, caseData.Id);
            //var case3 = client.GetCase(caseWorkerToken, caseData.Id);
            //var case4 = client.GetCase(caseWorkerToken, caseData.Id);
            //var case5 = client.GetCase(caseWorkerToken, caseData.Id);

            Assert.AreEqual(CaseStateKind.New, case1.ReadonlyData.State.StateKind);
            Assert.AreEqual(CaseStateKind.PreliminaryComments, case2.ReadonlyData.State.StateKind);
            //Assert.AreEqual(CaseStateKind.PriliminaryDecision, case3.ReadonlyData.State.StateKind);
            //Assert.AreEqual(CaseStateKind.FinalDecision, case4.ReadonlyData.State.StateKind);
            //Assert.AreEqual(CaseStateKind.Closed, case5.ReadonlyData.State.StateKind);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [DllImport("kernel32.dll", EntryPoint = "GetSystemTime", SetLastError = true)]
        private static extern void Win32GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
        private static extern bool Win32SetSystemTime(ref SYSTEMTIME lpSystemTime);

        private static Integrator.CaseData CreateCase(int id)
        {
            var caseData = new Integrator.CaseData
            {
                Id = id,
                CaseManagerEmail = DefaultUser.Email,
                ClaimantContact = new Integrator.CaseData.ContactData
                {
                    Name = "name",
                    EMail = "claimant@email.com",
                    Address = "address",
                    Phone = "+375297775218",
                },
                StartDate = DateTime.Now,
            };
            var destination = new Destination(ConfigurationManager.AppSettings["WebApiUrl"]);
            destination.CreateCase(caseData);
            return caseData;
        }
    }
}