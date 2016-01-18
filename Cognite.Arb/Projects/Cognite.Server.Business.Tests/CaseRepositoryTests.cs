using System;
using Cognite.Arb.Server.Business.DDD;
using Moq;
using NUnit.Framework;

namespace Cognite.Server.Business.Tests
{
    [TestFixture]
    public class CaseRepositoryTests
    {
        private MockRepository _mockRepository;
        private Mock<CaseRepository.ICaseGateway> _databaseMock;
        private CaseRepository.ICaseGateway _caseGateway;
        private Mock<CaseRepository.IAssignmentSchedule> _assignmentScheduleMock;
        private CaseRepository.IAssignmentSchedule _assignmentSchedule;
        private Mock<CaseRepository.IUserGateway> _userGatewayMock;
        private CaseRepository.IUserGateway _userGateway;
        private CaseRepository _caseRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _databaseMock = _mockRepository.Create<CaseRepository.ICaseGateway>();
            _caseGateway = _databaseMock.Object;
            _assignmentScheduleMock = _mockRepository.Create<CaseRepository.IAssignmentSchedule>();
            _assignmentSchedule = _assignmentScheduleMock.Object;
            _userGatewayMock = _mockRepository.Create<CaseRepository.IUserGateway>();
            _userGateway = _userGatewayMock.Object;
            _caseRepository = new CaseRepository(_caseGateway, _userGateway, _assignmentSchedule);
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.VerifyAll();
        }

        #region Create

        [Test]
        public void CanCreateEmptyCase()
        {
            // Arrange

            var createCaseData = new CreateCaseData
            {
                Id = 1,
                CaseManagerEmail = null,
                Claimant = null,
                StartDate = null,
            };
            _databaseMock.Setup(o => o.GetById(createCaseData.Id)).Returns((Case) null);
            _userGatewayMock.Setup(o => o.GetByEmail(createCaseData.CaseManagerEmail)).Returns((User) null);
            _assignmentScheduleMock.Setup(o => o.GetNextAssignment()).Returns((CaseAssignedUserCollection.PanelMemberCollection) null);
            Case savedCase = null;
            _databaseMock.Setup(o => o.Create(It.IsAny<Case>())).Callback<Case>(r => savedCase = r);

            // Act

            _caseRepository.Create(createCaseData);

            // Assert

            Assert.That(savedCase.Id == createCaseData.Id);
            Assert.IsNull(savedCase.Description.StartDate);
            Assert.IsNotNull(savedCase.Contacts);
            Assert.IsNull(savedCase.Contacts.Relationship);
            Assert.IsNull(savedCase.Contacts.ContactAgreement);
            Assert.IsNull(savedCase.Contacts.Claimant);
            Assert.IsNull(savedCase.Contacts.Architect);
            Assert.IsNull(savedCase.AssignedUsers.CaseWorker);
            Assert.IsNotNull(savedCase.AssignedUsers.PanelMembers);
            Assert.IsNull(savedCase.AssignedUsers.PanelMembers.First);
            Assert.IsNull(savedCase.AssignedUsers.PanelMembers.Second);
            Assert.IsNull(savedCase.AssignedUsers.PanelMembers.Third);
        }

        [Test]
        public void CanCreateFilledCase()
        {
            // Arrange

            var createCaseData = new CreateCaseData
            {
                Id = 1,
                CaseManagerEmail = null,
                Claimant = new CreateCaseData.ClaimantData
                {
                    Address = "address",
                    EMail = "email",
                    Name = "name",
                    Phone = "phone",
                },
                StartDate = DateTime.Now,
            };
            _databaseMock.Setup(o => o.GetById(createCaseData.Id)).Returns((Case)null);
            _userGatewayMock.Setup(o => o.GetByEmail(createCaseData.CaseManagerEmail)).Returns((User)null);
            _assignmentScheduleMock.Setup(o => o.GetNextAssignment()).Returns((CaseAssignedUserCollection.PanelMemberCollection)null);
            Case savedCase = null;
            _databaseMock.Setup(o => o.Create(It.IsAny<Case>())).Callback<Case>(r => savedCase = r);

            // Act

            _caseRepository.Create(createCaseData);

            // Assert

            Assert.That(savedCase.Id == createCaseData.Id);
            Assert.AreEqual(createCaseData.StartDate, savedCase.Description.StartDate);
            Assert.IsNotNull(savedCase.Contacts);
            Assert.IsNull(savedCase.Contacts.Relationship);
            Assert.IsNull(savedCase.Contacts.ContactAgreement);
            Assert.AreEqual(createCaseData.Claimant.Address, savedCase.Contacts.Claimant.Address);
            Assert.AreEqual(createCaseData.Claimant.EMail, savedCase.Contacts.Claimant.EMail);
            Assert.AreEqual(createCaseData.Claimant.Name, savedCase.Contacts.Claimant.Name);
            Assert.AreEqual(createCaseData.Claimant.Phone, savedCase.Contacts.Claimant.Phone);
            Assert.IsNull(savedCase.Contacts.Architect);
            Assert.IsNull(savedCase.AssignedUsers.CaseWorker);
            Assert.IsNotNull(savedCase.AssignedUsers.PanelMembers);
            Assert.IsNull(savedCase.AssignedUsers.PanelMembers.First);
            Assert.IsNull(savedCase.AssignedUsers.PanelMembers.Second);
            Assert.IsNull(savedCase.AssignedUsers.PanelMembers.Third);
        }

        [Test, ExpectedException(typeof (CaseRepository.DuplicateException))]
        public void CannotCreateDuplicateCase()
        {
            // Arrange

            var createCaseData = new CreateCaseData
            {
                Id = 1,
                CaseManagerEmail = null,
                Claimant = null,
                StartDate = null,
            };
            _databaseMock.Setup(o => o.GetById(createCaseData.Id)).Returns(new Case());

            // Act

            _caseRepository.Create(createCaseData);
        }

        [Test]
        public void CanAutomaticallyAssignPanelMembers()
        {
            // Arrange

            var createCaseData = new CreateCaseData
            {
                Id = 1,
                CaseManagerEmail = null,
                Claimant = null,
                StartDate = null,
            };
            var assignment = new CaseAssignedUserCollection.PanelMemberCollection
            {
                First = new User {Role = User.RoleKind.PanelMember},
                Second = new User {Role = User.RoleKind.PanelMember},
                Third = new User {Role = User.RoleKind.PanelMember},
            };
            _databaseMock.Setup(o => o.GetById(createCaseData.Id)).Returns((Case) null);
            _userGatewayMock.Setup(o => o.GetByEmail(createCaseData.CaseManagerEmail)).Returns((User) null);
            _assignmentScheduleMock.Setup(o => o.GetNextAssignment()).Returns(assignment);
            Case savedCase = null;
            _databaseMock.Setup(o => o.Create(It.IsAny<Case>())).Callback<Case>(r => savedCase = r);

            // Act

            _caseRepository.Create(createCaseData);

            // Assert

            Assert.AreEqual(assignment.First, savedCase.AssignedUsers.PanelMembers.First);
            Assert.AreEqual(assignment.Second, savedCase.AssignedUsers.PanelMembers.Second);
            Assert.AreEqual(assignment.Third, savedCase.AssignedUsers.PanelMembers.Third);
        }

        [Test]
        public void CanAutomaticallyAssignCaseWorker()
        {
            // Arrange

            var createCaseData = new CreateCaseData
            {
                Id = 1,
                CaseManagerEmail = "caseworker@test.com",
                Claimant = null,
                StartDate = null,
            };
            var caseWorker = new User {Role = User.RoleKind.CaseWorker};
            _databaseMock.Setup(o => o.GetById(createCaseData.Id)).Returns((Case) null);
            _userGatewayMock.Setup(o => o.GetByEmail(createCaseData.CaseManagerEmail)).Returns(caseWorker);
            _assignmentScheduleMock.Setup(o => o.GetNextAssignment()).Returns((CaseAssignedUserCollection.PanelMemberCollection) null);
            Case savedCase = null;
            _databaseMock.Setup(o => o.Create(It.IsAny<Case>())).Callback<Case>(r => savedCase = r);

            // Act

            _caseRepository.Create(createCaseData);

            // Assert

            Assert.AreEqual(caseWorker, savedCase.AssignedUsers.CaseWorker);
        }

        [Test]
        public void CaseWorkerIsNotAssignedIfCaseManagerIsNotACaseWorker()
        {
            // Arrange

            var createCaseData = new CreateCaseData
            {
                Id = 1,
                CaseManagerEmail = "caseworker@test.com",
                Claimant = null,
                StartDate = null,
            };
            var caseWorker = new User { Role = User.RoleKind.Admin };
            _databaseMock.Setup(o => o.GetById(createCaseData.Id)).Returns((Case)null);
            _userGatewayMock.Setup(o => o.GetByEmail(createCaseData.CaseManagerEmail)).Returns(caseWorker);
            _assignmentScheduleMock.Setup(o => o.GetNextAssignment()).Returns((CaseAssignedUserCollection.PanelMemberCollection) null);
            Case savedCase = null;
            _databaseMock.Setup(o => o.Create(It.IsAny<Case>())).Callback<Case>(r => savedCase = r);

            // Act

            _caseRepository.Create(createCaseData);

            // Assert

            Assert.IsNull(savedCase.AssignedUsers.CaseWorker);
        }

        #endregion

        [Test]
        public void CanGetNullCaseById()
        {
            // Arrange
            
            const int caseId = 10;
            _databaseMock.Setup(o => o.GetById(caseId)).Returns((Case) null);

            // Act

            var @case = _caseRepository.GetById(caseId);

            // Assert

            Assert.IsNull(@case);
        }

        [Test]
        public void CanGetCaseById()
        {
            // Arrange

            const int caseId = 10;
            var @existingCase = new Case();
            _databaseMock.Setup(o => o.GetById(caseId)).Returns(existingCase);

            // Act

            var @case = _caseRepository.GetById(caseId);

            // Assert

            Assert.AreEqual(existingCase, @case);

        }
    }
}