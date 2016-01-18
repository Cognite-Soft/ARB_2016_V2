using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Cognite.Arb.Server.Business.Cases;
using Cognite.Arb.Server.Business.Database;
using Cognite.Arb.Server.Business.Mailing;
using Cognite.Arb.Server.Contract;
using Cognite.Arb.Server.Contract.Cases;
using Allegation = Cognite.Arb.Server.Business.Cases.Allegation;
using AllegationCollection = Cognite.Arb.Server.Business.Cases.AllegationCollection;
using AllegationCommentType = Cognite.Arb.Server.Business.Cases.AllegationCommentType;
using ArchitectContact = Cognite.Arb.Server.Business.Cases.ArchitectContact;
using CaseContacts = Cognite.Arb.Server.Business.Cases.CaseContacts;
using CaseHeader = Cognite.Arb.Server.Business.Database.CaseHeader;
using CasePanelMembers = Cognite.Arb.Server.Business.Cases.CasePanelMembers;
using CaseState = Cognite.Arb.Server.Business.Cases.CaseState;
using CaseStateKind = Cognite.Arb.Server.Business.Cases.CaseStateKind;
using CaseUpdate = Cognite.Arb.Server.Business.Cases.CaseUpdate;
using Contact = Cognite.Arb.Server.Business.Cases.Contact;
using DateAndDetail = Cognite.Arb.Server.Business.Cases.DateAndDetail;
using Document = Cognite.Arb.Server.Business.Cases.Document;
using FinalDecisionComment = Cognite.Arb.Server.Business.Cases.FinalDecisionComment;
using InitialCaseData = Cognite.Arb.Server.Business.Cases.InitialCaseData;
using MyAllegationComment = Cognite.Arb.Server.Business.Cases.MyAllegationComment;
using NewDocument = Cognite.Arb.Server.Contract.Cases.NewDocument;
using NewPartiesComment = Cognite.Arb.Server.Business.Cases.NewPartiesComment;
using PanelMembersUpdate = Cognite.Arb.Server.Business.Cases.PanelMembersUpdate;
using Question = Cognite.Arb.Server.Business.Cases.Question;
using ReadonlyCaseData = Cognite.Arb.Server.Business.Cases.ReadonlyCaseData;
using ResetToken = Cognite.Arb.Server.Business.Database.ResetToken;
using Role = Cognite.Arb.Server.Business.Database.Role;
using Schedule = Cognite.Arb.Server.Business.Database.Schedule;
using User = Cognite.Arb.Server.Business.Mailing.User;

namespace Cognite.Arb.Server.Business
{
    public class Facade
    {
        private readonly IDatabase _database;
        private readonly IConfiguration _configuration;
        private readonly IPasswordManager _passwordManager;
        private readonly IMailNotifier _mailNotifier;
        private readonly ISecurityTokenService _securityTokenService;
        private readonly IDocumentStore _documentStore;

        public Facade(IDatabase database, IConfiguration configuration, IPasswordManager passwordManager,
            ISecurityTokenService securityTokenService, IMailNotifier mailNotifier, IDocumentStore documentStore)
        {
            _database = database;
            _configuration = configuration;
            _passwordManager = passwordManager;
            _securityTokenService = securityTokenService;
            _mailNotifier = mailNotifier;
            _documentStore = documentStore;
        }

        #region Public

        public SecurePhraseQuestion StartLoginAndGetSecurePhraseQuestion(Login login)
        {
            var userData = _database.GetUserByEmail(login.Email);
            if (userData == null) throw new InvalidUserOrPasswordException();
            if (userData.UserState != UserState.Activated) throw new InvalidUserOrPasswordException();
            if (_passwordManager.HashPassword(login.Password, userData.PasswordSalt) != userData.HashedPassword)
                throw new InvalidUserOrPasswordException();
            var securityToken = _securityTokenService.AddLogin(userData);
            return new SecurePhraseQuestion
            {
                SecurityToken = securityToken,
                FirstCharacterIndex = userData.FirstSecurePhraseQuestionCharacterIndex.Value,
                SecondCharacterIndex = userData.SecondSecurePhraseQuestionCharacterIndex.Value,
            };
        }

        public AuthenticationResult FinishLoginWithSecurePhraseAnswer(string token, SecurePhraseAnswer answer)
        {
            var user = _securityTokenService.GetLogin(token);
            if (user == null) throw new NotAuthenticatedException();
            using (var transaction = new TransactionScope())
            {
                var userData = _database.GetUserByEmail(user.Email);
                if (userData == null || userData.UserState != UserState.Activated) throw new UserDoesNotExistException();
                var securePhrase = _passwordManager.DecryptSecurePhrase(userData.EncryptedSecurePhrase);
                if (securePhrase[userData.FirstSecurePhraseQuestionCharacterIndex.Value] != answer.FirstCharacter
                    || securePhrase[userData.SecondSecurePhraseQuestionCharacterIndex.Value] != answer.SecondCharacter)
                    throw new InvalidSecurePhraseAnswerException();
                var indexes = _passwordManager.GetSecurePhraseQuestion(securePhrase.Length);
                userData.FirstSecurePhraseQuestionCharacterIndex = indexes.Item1;
                userData.SecondSecurePhraseQuestionCharacterIndex = indexes.Item2;
                _database.UpdateUser(userData);
                _securityTokenService.RemoveLogin(token);
                var securityToken = _securityTokenService.Add(user);
                UpdateCaseStatuses();
                transaction.Complete();
                return new AuthenticationResult
                {
                    SecurityToken = securityToken,
                    Role = user.Role,
                };
            }
        }

        private void UpdateCaseStatuses()
        {
            var minimumStartDate = DateTime.Today.AddDays(-(4 + 3 + 5)*7);
            var caseIds = _database.GetOpenCasesOlderThan(minimumStartDate);
            _database.SetClosedState(caseIds);
        }

        public void Logout(string securityToken)
        {
            _securityTokenService.Remove(securityToken);
        }

        public void InitiateResetPassword(string email)
        {
            using (var transaction = new TransactionScope())
            {
                var userData = _database.GetUserByEmail(email);
                if (userData == null || userData.UserState == UserState.Deleted) throw new UserDoesNotExistException();
                _database.DeleteExpiredResetTokens();
                _database.DeleteResetTokenIfExists(userData.Id, ResetTokenType.Password);
                var resetToken = new ResetToken
                {
                    UserId = userData.Id,
                    Token = Guid.NewGuid().ToString(),
                    ExpirationTime = DateTime.Now.Add(_configuration.ResetPasswordTokenLifespan),
                    Type = ResetTokenType.Password,
                };
                _database.AddResetToken(resetToken);
                transaction.Complete();
                _mailNotifier.SendResetPasswordInstruction(new ResetNotification
                {
                    UserId = userData.Id,
                    UserEmail = userData.Email,
                    UserFirstName = userData.FirstName,
                    UserLastName = userData.LastName,
                    ResetToken = resetToken.Token,
                    ExpirationTime = resetToken.ExpirationTime,
                });
            }
        }

        public void InitiateResetSecurePhrase(string email)
        {
            using (var transaction = new TransactionScope())
            {
                var userData = _database.GetUserByEmail(email);
                if (userData == null || userData.UserState == UserState.Deleted) throw new UserDoesNotExistException();
                _database.DeleteExpiredResetTokens();
                _database.DeleteResetTokenIfExists(userData.Id, ResetTokenType.SecurePhrase);
                var resetToken = new ResetToken
                {
                    UserId = userData.Id,
                    Token = Guid.NewGuid().ToString(),
                    ExpirationTime = DateTime.Now.Add(_configuration.ResetSecurePhraseTokenLifespan),
                    Type = ResetTokenType.SecurePhrase,
                };
                _database.AddResetToken(resetToken);
                transaction.Complete();
                _mailNotifier.SendResetSecurePhraseInstruction(new ResetNotification
                {
                    UserId = userData.Id,
                    UserEmail = userData.Email,
                    UserFirstName = userData.FirstName,
                    UserLastName = userData.LastName,
                    ResetToken = resetToken.Token,
                    ExpirationTime = resetToken.ExpirationTime,
                });
            }
        }

        public ResetTokenInfo ValidateResetToken(string resetToken)
        {
            _database.DeleteExpiredResetTokens();
            var token = _database.GetResetToken(resetToken);
            if (token == null) throw new InvalidResetTokenException();
            var userData = _database.GetUserById(token.UserId);
            if (userData == null || userData.UserState == UserState.Deleted) throw new UserDoesNotExistException();
            return new ResetTokenInfo
            {
                User = userData,
                Type = token.Type,
            };
        }

        public void FinishResetPassword(string resetToken, string password)
        {
            if (!_passwordManager.ValidatePasswordStrength(password)) throw new WeakPasswordException();
            using (var transaction = new TransactionScope())
            {
                var userData = GetUserByResetToken(resetToken);
                if (userData == null) throw new InvalidResetTokenException();
                userData.PasswordSalt = _passwordManager.GenerateSalt();
                userData.HashedPassword = _passwordManager.HashPassword(password, userData.PasswordSalt);
                _database.UpdateUser(userData);
                transaction.Complete();
                _mailNotifier.SendResetPasswordComplete(new User
                {
                    UserId = userData.Id,
                    Email = userData.Email,
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                });
            }
        }

        public void FinishResetSecurePhrase(string resetToken, string securePhrase)
        {
            if (!_passwordManager.ValidateSecurePhraseStrength(securePhrase)) throw new WeakSecurePhraseException();
            using (var transaction = new TransactionScope())
            {
                var userData = GetUserByResetToken(resetToken);
                if (userData == null) throw new InvalidResetTokenException();
                userData.EncryptedSecurePhrase = _passwordManager.EncryptSecurePhrase(securePhrase);
                _database.UpdateUser(userData);
                transaction.Complete();
                _mailNotifier.SendResetSecurePhraseComplete(new User
                {
                    UserId = userData.Id,
                    Email = userData.Email,
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                });
            }
        }

        public void FinishActivateUser(string resetToken, string password, string securePhrase)
        {
            if (!_passwordManager.ValidatePasswordStrength(password)) throw new WeakPasswordException();
            if (!_passwordManager.ValidateSecurePhraseStrength(securePhrase)) throw new WeakSecurePhraseException();
            using (var transaction = new TransactionScope())
            {
                var userData = GetUserByResetToken(resetToken);
                if (userData == null) throw new InvalidResetTokenException();
                userData.PasswordSalt = _passwordManager.GenerateSalt();
                userData.HashedPassword = _passwordManager.HashPassword(password, userData.PasswordSalt);
                userData.EncryptedSecurePhrase = _passwordManager.EncryptSecurePhrase(securePhrase);
                userData.UserState = UserState.Activated;
                var question = _passwordManager.GetSecurePhraseQuestion(securePhrase.Length);
                userData.FirstSecurePhraseQuestionCharacterIndex = question.Item1;
                userData.SecondSecurePhraseQuestionCharacterIndex = question.Item2;
                _database.UpdateUser(userData);
                _database.DeleteResetTokenIfExists(userData.Id, ResetTokenType.Both);
                transaction.Complete();
                _mailNotifier.SendNewUserComplete(new User
                {
                    UserId = userData.Id,
                    Email = userData.Email,
                    FirstName = userData.FirstName,
                    LastName = userData.LastName,
                });
            }
        }

        public bool CheckPasswordComplexity(string password)
        {
            if (_passwordManager.ValidatePasswordStrength(password))
                return true;
            return false;
        }

        public UserHeader GetUserByToken(string token)
        {
            var user = _securityTokenService.Get(token);
            if (user == null) throw new SecurityTokenNotFoundException();
            return user;
        }

        public UserHeader GetUserById(Guid id)
        {
            var result = _database.GetUserById(id);
            if (result == null) throw new UserDoesNotExistException();
            return result;
        }

        public UserHeader[] GetUsers()
        {
            return _database.GetActiveUsers();
        }

        public UserHeader CreateUser(UserHeader user)
        {
            using (var transaction = new TransactionScope())
            {
                var userData = _database.GetUserByEmail(user.Email);
                if (userData != null)
                {
                    if (userData.UserState != UserState.Deleted) throw new DuplicateUserException();
                    userData.UserState = UserState.Created;
                    userData.FirstName = user.FirstName;
                    userData.LastName = user.LastName;
                    userData.HashedPassword = null;
                    userData.PasswordSalt = null;
                    userData.EncryptedSecurePhrase = null;
                    userData.FirstSecurePhraseQuestionCharacterIndex = null;
                    userData.SecondSecurePhraseQuestionCharacterIndex = null;
                    userData.Role = user.Role;
                    _database.UpdateUser(userData);
                }
                else
                {
                    userData = new UserData(user);
                    if (userData.Id == Guid.Empty) userData.Id = Guid.NewGuid();
                    _database.CreateUser(userData);
                }
                var resetToken = new ResetToken
                {
                    UserId = userData.Id,
                    Token = Guid.NewGuid().ToString(),
                    ExpirationTime = DateTime.Now.Add(_configuration.ResetUserTokenLifespan),
                    Type = ResetTokenType.Both,
                };
                _database.DeleteExpiredResetTokens();
                _database.DeleteResetTokenIfExists(userData.Id, ResetTokenType.Both);
                _database.AddResetToken(resetToken);
                transaction.Complete();
                _mailNotifier.SendNewUserInstruction(new ResetNotification
                {
                    UserId = userData.Id,
                    UserEmail = userData.Email,
                    UserFirstName = userData.FirstName,
                    UserLastName = userData.LastName,
                    ResetToken = resetToken.Token,
                    ExpirationTime = resetToken.ExpirationTime,
                });
                return userData;
            }
        }

        public void UpdateUser(UserHeader user)
        {
            using (var transaction = new TransactionScope())
            {
                var userData = _database.GetUserByEmail(user.Email);
                if (userData == null) throw new UserDoesNotExistException();
                userData.FirstName = user.FirstName;
                userData.LastName = user.LastName;
                userData.Role = user.Role;
                _database.UpdateUser(userData);
                transaction.Complete();
            }
        }

        public void DeleteUser(Guid id)
        {
            using (var transaction = new TransactionScope())
            {
                var userData = _database.GetUserById(id);
                if (userData == null) throw new UserDoesNotExistException();
                if (GetIsLooseUser(id))
                {
                    _database.DeleteUser(id);
                }
                else
                {
                    var assignments = _database.GetUserAssignments(id);
                    foreach (var assignment in assignments)
                        _database.DisassignUser(assignment, id);
                    userData.UserState = UserState.Deleted;
                    _database.UpdateUser(userData);
                }
                transaction.Complete();
            }
        }

        public void CreateCase(CreateCaseInfo @case, string caseWorkerEmail)
        {
            using (var transaction = new TransactionScope())
            {
                var caseWorker = _database.GetUserByEmail(caseWorkerEmail);

                var existingCase = _database.GetCaseById(@case.Id);
                if (existingCase != null) throw new DuplicateCaseIdException();

                var newCase = new CaseData
                {
                    Id = @case.Id,
                    ParentId = null,
                    Status = CaseData.CaseStatus.New,
                    Background = null,
                    IdealOutcome = null,
                    Relationship = null,
                    ContactAgreement = null,
                    StartDate = @case.StartDate,
                    IssueRaisedWithArchitect = new CaseData.Question(),
                    SubjectOfLegalProceedings = new CaseData.Question(),
                    ArchitectContact = new CaseData.ArchitectContactData(),
                    ClaimantContact = new CaseData.ContactData
                    {
                        Name = @case.ClaimantContact.Name,
                        Address = @case.ClaimantContact.Address,
                        EMail = @case.ClaimantContact.EMail,
                        Phone = @case.ClaimantContact.Phone,
                    },
                };
                _database.CreateCase(newCase);
                if (caseWorker != null)
                    AssignCaseWorker(newCase.Id, caseWorker);
                AutoAssignPanelMembers(newCase);

                transaction.Complete();
            }
        }

        public Cases.Case GetCase(UserHeader user, int id)
        {
            var @case = _database.GetCaseById(id);
            if (@case == null) throw new CaseDoesNotExistsException();
            if (!CanSeeAllCases(user) && !IsUserAssignedToCase(user, @case)) throw new ForbiddenException();
            var state = GetCaseState(@case);
            var partiesComments = _database.GetPartiesComments(id);
            var result = new Cases.Case
            {
                ReadonlyData = new ReadonlyCaseData
                {
                    Id = @case.Id,
                    ParentId = @case.ParentId,
                    State = state,
                },
                InitialData = new InitialCaseData
                {
                    CanBeEdited = state.StateKind == CaseStateKind.New,
                    IsReady = GetIsCaseReadyToStart(@case),
                    StartDate = @case.StartDate,
                    Background = @case.Background,
                    IdealOutcome = @case.IdealOutcome,
                    IssueRaisedWithArchitect = new Question
                    {
                        Answer = @case.IssueRaisedWithArchitect.Answer,
                        Comments = @case.IssueRaisedWithArchitect.Comments,
                    },
                    SubjectOfLegalProceedings = new Question
                    {
                        Answer = @case.SubjectOfLegalProceedings.Answer,
                        Comments = @case.SubjectOfLegalProceedings.Comments,
                    },
                    CasePanelMembers = GetPanelMembers(@case),
                },
                Contacts = new CaseContacts
                {
                    Relationship = @case.Relationship,
                    ContactAgreement = @case.ContactAgreement,
                    ArchitectContact = new ArchitectContact
                    {
                        Name = @case.ArchitectContact.Name,
                        Address = @case.ArchitectContact.Address,
                        EMail = @case.ArchitectContact.EMail,
                        Phone = @case.ArchitectContact.Phone,
                        RegistrationNumber = @case.ArchitectContact.RegistrationNumber,
                    },
                    ClaimantContact = new Contact
                    {
                        Name = @case.ClaimantContact.Name,
                        Address = @case.ClaimantContact.Address,
                        EMail = @case.ClaimantContact.EMail,
                        Phone = @case.ClaimantContact.Phone,
                    },
                },
                DatesAndDetails = @case.DatesAndDetails.Select(item => new DateAndDetail
                {
                    Id = item.Id,
                    Date = item.Date,
                    Text = item.Text,
                    Documents = item.Documents.Select(d => new Document {Id = d.Id, Name = d.Name}).ToArray(),
                }).ToArray(),
                Allegations = new AllegationCollection
                {
                    CanAddAllegation = IsNewOrPreliminaryComments(state),
                    Items = @case.Allegations.Select(item => new Allegation
                    {
                        Id = item.Id,
                        Text = item.Text,
                        MyComment = item.Comments.Where(c => c.PanelMemberId == user.Id).Select(c =>
                            new MyAllegationComment
                            {
                                AllegationCommentType = (AllegationCommentType) c.CommentType,
                                Text = c.Text,
                                AdditionalText = c.AdditionalText,
                            }).FirstOrDefault(),
                        Documents = item.Documents.Select(d => new Document {Id = d.Id, Name = d.Name}).ToArray(),
                        CanBeDeleted = !item.Comments.Any() && IsNewOrPreliminaryComments(state),
                    }).ToArray(),
                },
                PartiesComments = partiesComments,
            };

            result.PreliminaryDecisionDocument = @case.PreliminaryDecisionDocument == null
                ? null
                : new Document {Id = @case.PreliminaryDecisionDocument.Id, Name = @case.PreliminaryDecisionDocument.Name};
            result.FinalDecisionDocument = @case.FinalDecisionDocument == null
                ? null
                : new Document {Id = @case.FinalDecisionDocument.Id, Name = @case.FinalDecisionDocument.Name};

            return result;
        }

        private bool GetIsCaseReadyToStart(CaseData @case)
        {
            return !(@case.StartDate == null
                     || string.IsNullOrEmpty(@case.Background) || string.IsNullOrEmpty(@case.IdealOutcome)
                     || IsQuestionNotAnswered(@case.IssueRaisedWithArchitect)
                     || IsQuestionNotAnswered(@case.SubjectOfLegalProceedings));
        }

        private bool IsQuestionNotAnswered(CaseData.Question question)
        {
            return question == null || question.Answer == null || question.Comments == null;
        }

        private static bool IsNewOrPreliminaryComments(CaseState state)
        {
            return state.StateKind == CaseStateKind.New || state.StateKind == CaseStateKind.PriliminaryComments;
        }

        private CasePanelMembers GetPanelMembers(CaseData caseData)
        {
            var panelMembers = caseData.AssignedUsers.Where(user => user.Role == Role.PanelMember).ToArray();

            return new CasePanelMembers
            {
                PanelMember1 = panelMembers.FirstOrDefault(),
                PanelMember2 = panelMembers.Skip(1).FirstOrDefault(),
                PanelMember3 = panelMembers.Skip(2).FirstOrDefault(),
            };
        }

        private CaseState GetCaseState(CaseData caseData)
        {
            if (!caseData.StartDate.HasValue && caseData.Status != CaseData.CaseStatus.New)
                throw new Exception("incorrect invariant: case start date can't be null when status is not draft");
            const int weeks = 7;
            var status = caseData.Status;
            var startDate = caseData.StartDate.Value;
            switch (status)
            {
                case CaseData.CaseStatus.New:
                {
                    var dueBy = startDate == null ? (DateTime?) null : startDate.AddDays(4*weeks);
                    var daysLeft = startDate == null ? null : (int?) (dueBy.Value - DateTime.Today).TotalDays;
                    return new CaseState
                    {
                        StateKind = CaseStateKind.New,
                        DueByDate = dueBy,
                        DueDaysLeft = daysLeft,
                    };
                }
                case CaseData.CaseStatus.Open:
                {
                    var today = DateTime.Today;
                    var timeSpan = today - startDate;
                    var totalDays = timeSpan.TotalDays;
                    if (totalDays <= 4*weeks)
                    {
                        var dueBy = startDate.AddDays(4*weeks);
                        return new CaseState
                        {
                            StateKind = CaseStateKind.PriliminaryComments,
                            DueByDate = dueBy,
                            DueDaysLeft = (int?) (dueBy - today).TotalDays,
                        };
                    }
                    else if (totalDays <= 4*weeks + 1*weeks)
                    {
                        var dueBy = startDate.AddDays(4*weeks + 1*weeks);
                        return new CaseState
                        {
                            StateKind = CaseStateKind.PriliminaryDecision,
                            DueByDate = dueBy,
                            DueDaysLeft = (int?) (dueBy - today).TotalDays,
                        };
                    }
                    else if (totalDays <= 4*weeks + 3*weeks)
                    {
                        var dueBy = startDate.AddDays(4*weeks + 3*weeks);
                        return new CaseState
                        {
                            StateKind = CaseStateKind.WaitingForPartyComments,
                            DueByDate = dueBy,
                            DueDaysLeft = (int?) (dueBy - today).TotalDays,
                        };
                    }
                    else if (totalDays <= 4*weeks + 3*weeks + 1*weeks)
                    {
                        var dueBy = startDate.AddDays(4*weeks + 3*weeks + 1*weeks);
                        return new CaseState
                        {
                            StateKind = CaseStateKind.FinalDecision,
                            DueByDate = dueBy,
                            DueDaysLeft = (int?) (dueBy - today).TotalDays,
                        };
                    }
                    else // if (totalDays < 4 * weeks + 3 * weeks + 5 * weeks)
                    {
                        var dueBy = startDate.AddDays(4*weeks + 3*weeks + 5*weeks);
                        return new CaseState
                        {
                            StateKind = CaseStateKind.Locked,
                            DueByDate = dueBy,
                            DueDaysLeft = (int?) (dueBy - today).TotalDays,
                        };
                    }
                }
                case CaseData.CaseStatus.Closed:
                    return new CaseState
                    {
                        StateKind = CaseStateKind.Closed,
                        DueByDate = null,
                        DueDaysLeft = null,
                    };
                case CaseData.CaseStatus.Rejected:
                    return new CaseState
                    {
                        StateKind = CaseStateKind.Rejected,
                        DueByDate = null,
                        DueDaysLeft = null,
                    };
                default:
                    throw new Exception("Unknown case status");
            }
        }

        public void UpdateCase(UserHeader user, int caseId, CaseUpdate caseUpdate)
        {
            using (var transaction = new TransactionScope())
            {
                var @case = _database.GetCaseById(caseId);
                if (@case.AssignedUsers.All(u => u.Id != user.Id)) throw new ForbiddenException();
                var state = GetCaseState(@case);
                if (caseUpdate.Initial != null && state.StateKind == CaseStateKind.New)
                {
                    @case.StartDate = caseUpdate.Initial.StartDate;
                    @case.Background = caseUpdate.Initial.Background;
                    @case.IdealOutcome = caseUpdate.Initial.IdealOutcome;
                    if (caseUpdate.Initial.IssueRaisedWithArchitect != null)
                    {
                        @case.IssueRaisedWithArchitect = new CaseData.Question
                        {
                            Answer = caseUpdate.Initial.IssueRaisedWithArchitect.Answer,
                            Comments = caseUpdate.Initial.IssueRaisedWithArchitect.Comments,
                        };
                    }
                    if (caseUpdate.Initial.SubjectOfLegalProceedings != null)
                    {
                        @case.SubjectOfLegalProceedings = new CaseData.Question
                        {
                            Answer = caseUpdate.Initial.SubjectOfLegalProceedings.Answer,
                            Comments = caseUpdate.Initial.SubjectOfLegalProceedings.Comments,
                        };
                    }
                }
                if (caseUpdate.Contacts != null)
                {
                    @case.ContactAgreement = caseUpdate.Contacts.ContactAgreement;
                    @case.Relationship = caseUpdate.Contacts.Relationship;
                    if (caseUpdate.Contacts.ArchitectContact != null)
                    {
                        @case.ArchitectContact.Name = caseUpdate.Contacts.ArchitectContact.Name;
                        @case.ArchitectContact.EMail = caseUpdate.Contacts.ArchitectContact.EMail;
                        @case.ArchitectContact.Address = caseUpdate.Contacts.ArchitectContact.Address;
                        @case.ArchitectContact.Phone = caseUpdate.Contacts.ArchitectContact.Phone;
                        @case.ArchitectContact.RegistrationNumber = caseUpdate.Contacts.ArchitectContact.RegistrationNumber;
                    }
                    if (caseUpdate.Contacts.ClaimantContact != null)
                    {
                        @case.ClaimantContact.Name = caseUpdate.Contacts.ClaimantContact.Name;
                        @case.ClaimantContact.EMail = caseUpdate.Contacts.ClaimantContact.EMail;
                        @case.ClaimantContact.Address = caseUpdate.Contacts.ClaimantContact.Address;
                        @case.ClaimantContact.Phone = caseUpdate.Contacts.ClaimantContact.Phone;
                    }
                }

                _database.UpdateCase(@case);

                if (caseUpdate.Initial != null && caseUpdate.Initial.PanelMembers != null)
                    UpdatePanelMembersAssignment(caseId, caseUpdate.Initial.PanelMembers, @case.AssignedUsers);

                if (caseUpdate.DatesAndDetailsUpdate != null)
                {
                    if (caseUpdate.DatesAndDetailsUpdate.NewDatesAndDetails != null)
                    {
                        foreach (var item in caseUpdate.DatesAndDetailsUpdate.NewDatesAndDetails)
                        {
                            int maxDateAndDetailsOrder = _database.GetMaxDateAndDetailOrder(caseId, item.Date);
                            _database.CreateDateAndDetail(caseId, item, user.Id, maxDateAndDetailsOrder + 1);
                            foreach (var document in item.Documents)
                            {
                                _documentStore.Upload(document.Id, caseId,
                                    new DocumentStoreItem {Name = document.Name, Content = document.Body});
                                _database.AddDocumentActivity(new DocumentActivity
                                {
                                    Id = Guid.NewGuid(),
                                    CaseId = @case.Id,
                                    DocumentId = document.Id,
                                    UserId = user.Id,
                                    ActionType = ActionType.Create,
                                    Date = DateTime.Now,
                                    DocumentType = DocumentType.DateAndDetail,
                                    DocumentName = document.Name,
                                });
                            }
                        }
                    }
                    if (caseUpdate.DatesAndDetailsUpdate.DeletedDatesAndDetails != null)
                    {
                        foreach (var item in caseUpdate.DatesAndDetailsUpdate.DeletedDatesAndDetails)
                        {
                            var dateAndDetailDocuments = _database.GetDateAndDetailDocument(item);
                            if (dateAndDetailDocuments != null)
                            {
                                foreach (var document in dateAndDetailDocuments)
                                {
                                    _documentStore.Delete(document.Id);
                                    _database.AddDocumentActivity(new DocumentActivity
                                    {
                                        Id = Guid.NewGuid(),
                                        CaseId = @case.Id,
                                        DocumentId = document.Id,
                                        UserId = user.Id,
                                        ActionType = ActionType.Delete,
                                        Date = DateTime.Now,
                                        DocumentType = DocumentType.DateAndDetail,
                                        DocumentName = document.Name,
                                    });
                                }
                            }
                            _database.DeleteDateAndDetail(item);
                        }
                    }
                }

                if (caseUpdate.AllegationsUpdate != null)
                {
                    if (state.StateKind != CaseStateKind.New && state.StateKind != CaseStateKind.PriliminaryComments)
                    {
                        if (caseUpdate.AllegationsUpdate.DeletedAllegations != null &&
                            caseUpdate.AllegationsUpdate.DeletedAllegations.Length > 0)
                            throw new ForbiddenException();
                        if (caseUpdate.AllegationsUpdate.NewAllegations != null &&
                            caseUpdate.AllegationsUpdate.NewAllegations.Length > 0)
                            throw new ForbiddenException();
                    }
                    if (caseUpdate.AllegationsUpdate.DeletedAllegations != null)
                        foreach (var item in caseUpdate.AllegationsUpdate.DeletedAllegations)
                        {
                            var documents = _database.GetAllegationDocuments(item);
                            foreach (var document in documents)
                            {
                                _documentStore.Delete(document.Id);
                            }
                            _database.DeleteAllegation(item);
                        }
                    if (caseUpdate.AllegationsUpdate.NewAllegations != null)
                    {
                        int order = 0;
                        foreach (var item in caseUpdate.AllegationsUpdate.NewAllegations)
                        {
                            _database.CreateAllegation(caseId, item, user.Id, order++);
                            foreach (var document in item.Documents)
                            {
                                _documentStore.Upload(document.Id, caseId, new DocumentStoreItem
                                {
                                    Name = document.Name,
                                    Content = document.Body,
                                });
                            }
                        }
                    }
                }

                transaction.Complete();
            }
        }

        private void UpdatePanelMembersAssignment(int caseId, PanelMembersUpdate panelMembers,
            IEnumerable<UserHeader> assignedUsers)
        {
            var panelMembersArray = new[] {panelMembers.PanelMember1, panelMembers.PanelMember2, panelMembers.PanelMember3}
                .Where(item => item != null).ToArray();
            var assignedPanelMembers = assignedUsers.Where(u => u.Role == Role.PanelMember).ToArray();

            var toUnassign = assignedPanelMembers.Where(a => panelMembersArray.All(n => n != a.Id)).Select(a => a.Id).ToArray();
            var toAssign = panelMembersArray.Where(n => assignedUsers.All(a => a.Id != n)).ToArray();

            foreach (var user in toUnassign)
                _database.DisassignUser(caseId, user);
            foreach (var user in toAssign)
                _database.AssignUser(caseId, user.Value);
        }

        public IEnumerable<CaseHeader> GetCaseHeaders(UserHeader user)
        {
            if (CanSeeAllCases(user))
                return _database.GetAllCaseHeaders();
            return _database.GetCaseHeadersByAssignedUserId(user.Id);
        }

        public int[] GetUserAssignments(Guid userId)
        {
            return _database.GetUserAssignments(userId);
        }

        public void UpdateUserAssignments(Guid userId, int[] newAssignments)
        {
            using (var transaction = new TransactionScope())
            {
                var oldAssignments = _database.GetUserAssignments(userId);
                var toCreate = newAssignments.Where(n => !oldAssignments.Any(o => o == n)).ToArray();
                var toDelete = oldAssignments.Where(o => !newAssignments.Any(n => n == o)).ToArray();
                var user = _database.GetUserById(userId);
                foreach (var caseId in toCreate)
                    CheckOverassignment(caseId, user.Role);
                CreateAndDeleteCaseAssignments(userId, toCreate, toDelete);
                _mailNotifier.NotifyAssignmentChange(new User
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserId = user.Id,
                }, toDelete, toCreate);
                transaction.Complete();
            }
        }

        public Schedule[] GetSchedules()
        {
            var schedules = _database.GetSchedules();
            var sortedSchedules = schedules.OrderBy(item => item.LastUsed ?? DateTime.MinValue).ThenBy(item => item.Id).ToArray();
            return sortedSchedules;
        }

        public void UpdateSchedule(int row, int column, Guid userId)
        {
            using (var transaction = new TransactionScope())
            {
                _database.GetSchedules();
                var schedule = _database.GetSchedule(row);
                if (schedule == null) throw new ArgumentException();
                var user = _database.GetUserById(userId);
                if (user == null) throw new UserDoesNotExistException();
                switch (column)
                {
                    case 0:
                        schedule.First = user;
                        break;
                    case 1:
                        schedule.Second = user;
                        break;
                    case 2:
                        schedule.Third = user;
                        break;
                    default:
                        throw new ArgumentException();
                }
                _database.UpdateSchedule(schedule);
                transaction.Complete();
            }
        }

        public void CommentAllegation(UserHeader user, Guid allegationId, string comment, AllegationCommentType type,
            string additionalComment)
        {
            if (user.Role != Role.PanelMember) throw new ForbiddenException();
            if (string.IsNullOrEmpty(comment)) throw new ArgumentException();
            if (type != AllegationCommentType.Advise && !string.IsNullOrEmpty(additionalComment)) throw new ArgumentException();
            //if (type == AllegationCommentType.Advise && string.IsNullOrEmpty(additionalComment)) throw new ArgumentException();

            var caseId = _database.GetCaseIdByAllegationId(allegationId);
            if (caseId == null) throw new CaseDoesNotExistsException();
            var @case = _database.GetCaseById(caseId.Value);
            var allegation = @case.Allegations.First(a => a.Id == allegationId);

            ForbidUnassignedUser(user, @case);
            ForbidNotPreliminaryCommentsCaseState(@case);
            ForbidSecondComment(user, allegation);

            _database.CreateAllegationComment(allegationId, comment, type, additionalComment, user.Id);

            RejectCaseIfAllAllegationsAreRejected(caseId.Value);
        }

        private void RejectCaseIfAllAllegationsAreRejected(int caseId)
        {
            var @case = _database.GetCaseById(caseId);
            var commentTypes = @case.Allegations.SelectMany(x => x.Comments.Select(y => y.CommentType));
            var allAnswered = @case.Allegations.All(x => x.Comments.Count() == 3);
            var allNo = commentTypes.All(x => x == CaseData.CommentType.No);
            if (allAnswered && allNo)
            {
                @case.Status = CaseData.CaseStatus.Rejected;
                _database.UpdateCase(@case);
            }
        }

        private static void ForbidSecondComment(UserHeader user, CaseData.Allegation allegation)
        {
            if (allegation.Comments.Any(c => c.PanelMemberId == user.Id)) throw new ForbiddenException();
        }

        private void ForbidNotPreliminaryCommentsCaseState(CaseData @case)
        {
            var status = GetCaseState(@case);
            if (status.StateKind != CaseStateKind.PriliminaryComments) throw new ForbiddenException();
        }

        private static void ForbidUnassignedUser(UserHeader user, CaseData @case)
        {
            if (@case.AssignedUsers.All(u => u.Id != user.Id)) throw new ForbiddenException();
        }

        public void StartCaseProcessing(UserHeader user, int caseId)
        {
            var @case = _database.GetCaseById(caseId);
            if (@case.AssignedUsers.All(u => u.Id != user.Id)) throw new ForbiddenException();
            var state = GetCaseState(@case);
            if (state.StateKind != CaseStateKind.New) throw new ForbiddenException();
            @case.Status = CaseData.CaseStatus.Open;
            @case.ProcessStartDate = DateTime.Now;
            _database.UpdateCase(@case);
        }

        public void AddPreliminaryDecisionComment(UserHeader user, int caseId, string text)
        {
            var @case = _database.GetCaseById(caseId);
            if (user.Role != Role.PanelMember) throw new ForbiddenException();
            if (@case.AssignedUsers.All(x => x.Id != user.Id)) throw new ForbiddenException();
            var comment = new PreliminaryDecisionComment
            {
                Id = Guid.NewGuid(),
                CaseId = caseId,
                PanelMemberId = user.Id,
                Text = text,
                Date = DateTime.Now,
            };
            _database.CreatePreliminaryDecisionComment(comment);
            var caseWorker = @case.AssignedUsers.FirstOrDefault(x => x.Role == Role.CaseWorker);
            _mailNotifier.NotifyCaseWorkerAboutPreliminaryDecisionComment(
                new User
                {
                    UserId = caseWorker.Id,
                    Email = caseWorker.Email,
                    FirstName = caseWorker.FirstName,
                    LastName = caseWorker.LastName
                },
                new User {UserId = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName},
                caseId, text);
        }

        public void AddFinalDecisionComment(UserHeader user, int caseId, FinalDecisionCommentKind decision, string text)
        {
            var @case = _database.GetCaseById(caseId);
            if (user.Role != Role.PanelMember) throw new ForbiddenException();
            if (@case.AssignedUsers.All(x => x.Id != user.Id)) throw new ForbiddenException();
            var comment = new FinalDecisionComment
            {
                Id = Guid.NewGuid(),
                CaseId = caseId,
                PanelMemberId = user.Id,
                Decision = decision,
                CommentForChange = text,
                Date = DateTime.Now,
            };
            _database.CreateFinalDecisionComment(comment);
            var caseWorker = @case.AssignedUsers.FirstOrDefault(x => x.Role == Role.CaseWorker);
            _mailNotifier.NotifyCaseWorkerAboutFinalDecisionComment(
                new User
                {
                    UserId = caseWorker.Id,
                    Email = caseWorker.Email,
                    FirstName = caseWorker.FirstName,
                    LastName = caseWorker.LastName
                },
                new User {UserId = user.Id, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName},
                caseId, text == null ? decision.ToString() : decision + ": " + text);
        }

        public void ReopenCase(int caseId)
        {
            using (var transaction = new TransactionScope())
            {
                var @case = _database.GetCaseById(caseId);
                if (@case == null) throw new CaseDoesNotExistException();
                var caseManager = @case.AssignedUsers.FirstOrDefault(x => x.Role == Role.CaseWorker);

                int nextCaseId = GetNextCaseId(caseId);

                var newCase = new CaseData
                {
                    Id = nextCaseId,
                    ParentId = caseId,
                    Status = CaseData.CaseStatus.New,
                    Background = @case.Background,
                    IdealOutcome = @case.IdealOutcome,
                    Relationship = @case.Relationship,
                    ContactAgreement = @case.ContactAgreement,
                    StartDate = DateTime.Now,
                    IssueRaisedWithArchitect = @case.IssueRaisedWithArchitect,
                    SubjectOfLegalProceedings = @case.SubjectOfLegalProceedings,
                    ArchitectContact = @case.ArchitectContact,
                    ClaimantContact = @case.ClaimantContact,
                };
                _database.CreateCase(newCase);
                if (caseManager != null)
                    AssignCaseWorker(newCase.Id, caseManager);
                AutoAssignPanelMembers(newCase);
                transaction.Complete();
            }
        }

        private int GetNextCaseId(int caseId)
        {
            for (int i = 1; i < 2000; ++i)
            {
                var result = caseId + 1000000*i;
                if (_database.GetCaseById(result) == null)
                    return result;
            }
            return caseId;
        }

        public void CloseCase(int caseId)
        {
            var @case = _database.GetCaseById(caseId);
            if (@case == null) throw new CaseDoesNotExistException();
            @case.Status = CaseData.CaseStatus.Closed;
            _database.UpdateCase(@case);
        }

        public void CloneCase(int caseId)
        {
            ReopenCase(caseId);
        }

        #endregion

        #region Activity Feed

        public ActivityFeed GetActivityFeed(int caseId)
        {
            var preliminaryComments = _database.GetPreliminaryCommentsActivityFeedItems(caseId);
            var preliminaryDecision = _database.GetPreliminaryDecisionActivityFeedItems(caseId);
            var finalDecision = _database.GetFinalDecisionActivityFeedItems(caseId);

            var @case = _database.GetCaseById(caseId);

            var documentActivities = _database.GetDocumentActivities(caseId);
            var posts = _database.GetPosts(caseId);
            var postItems = posts.Select(x => new ActivityFeed.ActivityFeedItem
            {
                Id = x.Id,
                Date = x.Date,
                User = x.User,
                Action = x.Parent == null
                    ? ActivityFeed.ActivityFeedAction.Discussion
                    : ActivityFeed.ActivityFeedAction.DiscussionComment,
                Description = x.Text,
            }).ToArray();
            var users = _database.GetAllUsers();
            var documentItems = documentActivities.Select(x => new ActivityFeed.ActivityFeedItem
            {
                Id = x.DocumentId,
                Date = x.Date,
                User = users.FirstOrDefault(u => u.Id == x.UserId),
                Action = GetDocumentAction(x.ActionType),
                Description = null,
            }).Where(x => x.User != null).ToArray();
            var allItems = postItems.Concat(documentItems).ToArray();
            var stagesAndDates = GetStagesAndDates(@case.StartDate ?? DateTime.Today);
            stagesAndDates[0].Items = new List<ActivityFeed.ActivityFeedItem>(preliminaryComments);
            stagesAndDates[1].Items = new List<ActivityFeed.ActivityFeedItem>(preliminaryDecision);
            stagesAndDates[2].Items = new List<ActivityFeed.ActivityFeedItem>(finalDecision);
            foreach (var item in allItems)
                foreach (var stage in stagesAndDates)
                    if (item.Date >= stage.StartDate && item.Date < stage.EndDate)
                        stage.Items.Add(item);

            var caseState = GetCaseState(@case);
            var sections = new List<ActivityFeed.ActivityFeedSection>
            {
                new ActivityFeed.ActivityFeedSection
                {
                    Header = new ActivityFeed.ActivityFeedSectionHeader
                    {
                        Title = "Final Decision",
                        DueBy =
                            caseState.StateKind == CaseStateKind.FinalDecision ||
                            caseState.StateKind == CaseStateKind.Locked
                                ? caseState.DueByDate
                                : null
                    },
                    SectionType = ActivityFeed.ActivityFeedSectionType.FinalDecision,
                    Items = stagesAndDates[2].Items.OrderByDescending(x => x.Date).ToArray(),
                    SectionStatus = GetSectionStatus(DateTime.Now, stagesAndDates[2]),
                },
                new ActivityFeed.ActivityFeedSection
                {
                    Header = new ActivityFeed.ActivityFeedSectionHeader
                    {
                        Title = "Preliminary Decision",
                        DueBy =
                            caseState.StateKind == CaseStateKind.PriliminaryDecision ||
                            caseState.StateKind == CaseStateKind.WaitingForPartyComments
                                ? caseState.DueByDate
                                : null
                    },
                    SectionType = ActivityFeed.ActivityFeedSectionType.PreliminaryDecision,
                    Items = stagesAndDates[1].Items.OrderByDescending(x => x.Date).ToArray(),
                    SectionStatus = GetSectionStatus(DateTime.Now, stagesAndDates[1]),
                },
                new ActivityFeed.ActivityFeedSection
                {
                    Header = new ActivityFeed.ActivityFeedSectionHeader
                    {
                        Title = "Preliminary Comments",
                        DueBy = caseState.StateKind == CaseStateKind.PriliminaryComments ? caseState.DueByDate : null
                    },
                    SectionType = ActivityFeed.ActivityFeedSectionType.PreliminaryComments,
                    Items = stagesAndDates[0].Items.OrderByDescending(x => x.Date).ToArray(),
                    SectionStatus =
                        caseState.StateKind == CaseStateKind.New
                            ? ActivityFeed.ActivityFeedSectionStatus.Future
                            : GetSectionStatus(DateTime.Now, stagesAndDates[0]),
                },
            };

            return new ActivityFeed
            {
                StartDate = @case.ProcessStartDate,
                Sections = sections.ToArray(),
            };
        }

        private ActivityFeed.ActivityFeedSectionStatus GetSectionStatus(DateTime now, StageAndDate stagesAndDate)
        {
            if (now < stagesAndDate.StartDate) return ActivityFeed.ActivityFeedSectionStatus.Future;
            if (now < stagesAndDate.EndDate) return ActivityFeed.ActivityFeedSectionStatus.Current;
            return ActivityFeed.ActivityFeedSectionStatus.Finished;
        }

        private ActivityFeed.ActivityFeedAction GetDocumentAction(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Create:
                    return ActivityFeed.ActivityFeedAction.CreateDocument;
                case ActionType.Update:
                    return ActivityFeed.ActivityFeedAction.UpdateDocument;
                case ActionType.Delete:
                    return ActivityFeed.ActivityFeedAction.DeleteDocument;
                default:
                    throw new ArgumentException();
            }
        }

        private enum CaseStage
        {
            PreliminaryComments,
            PreliminaryDecision,
            FinalDecision,
        }

        private class StageAndDate
        {
            public CaseStage Stage { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<ActivityFeed.ActivityFeedItem> Items { get; set; }
        }

        private StageAndDate[] GetStagesAndDates(DateTime startDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var preliminaryDecisionDate = startDate + TimeSpan.FromDays(4*7 + 1);
            var finalDecisionDate = preliminaryDecisionDate + TimeSpan.FromDays(3*7 + 1);

            return new[]
            {
                new StageAndDate
                {
                    Stage = CaseStage.PreliminaryComments,
                    StartDate = DateTime.MinValue,
                    EndDate = preliminaryDecisionDate,
                },
                new StageAndDate
                {
                    Stage = CaseStage.PreliminaryDecision,
                    StartDate = preliminaryDecisionDate,
                    EndDate = finalDecisionDate,
                },
                new StageAndDate
                {
                    Stage = CaseStage.FinalDecision,
                    StartDate = finalDecisionDate,
                    EndDate = DateTime.MaxValue,
                },
            };
        }

        #endregion

        #region Private

        private void CreateAndDeleteCaseAssignments(Guid userId, int[] toCreate, int[] toDelete)
        {
            foreach (var caseId in toCreate)
                _database.AssignUser(caseId, userId);
            foreach (var caseId in toDelete)
                _database.DisassignUser(caseId, userId);
        }

        private void CheckOverassignment(int caseId, Role role)
        {
            var assignedUsers = _database.GetAssignedUsers(caseId);
            var assignedUsersInRole = assignedUsers.Where(item => item.Role == role).ToArray();
            var count = assignedUsersInRole.Length;
            var maxCount = GetMaxUsersAssignedToCase(role);
            if (count >= maxCount) throw new OverAssignmentException(caseId, role, assignedUsersInRole);
        }

        private void AssignCaseWorker(int caseId, UserHeader caseWorker)
        {
            CheckOverassignment(caseId, Role.CaseWorker);
            _database.AssignUser(caseId, caseWorker.Id);
            _mailNotifier.NotifyAssignedCaseWorker(caseId, new User
            {
                UserId = caseWorker.Id,
                Email = caseWorker.Email,
                FirstName = caseWorker.FirstName,
                LastName = caseWorker.LastName,
            });
        }

        private void AutoAssignPanelMembers(CaseData caseData)
        {
            var panelMembersAssigned = caseData.AssignedUsers != null &&
                                       caseData.AssignedUsers.Any(u => u.Role == Role.PanelMember);
            if (panelMembersAssigned) return;
            var schedules = _database.GetSchedules();
            var filledSchedules = schedules.Where(s => s.First != null && s.Second != null && s.Third != null).ToArray();
            var orderedSchedules = filledSchedules.OrderBy(s => s.LastUsed).ToArray();
            var nextSchedule = orderedSchedules.FirstOrDefault();
            if (nextSchedule == null) return;
            _database.AssignUser(caseData.Id, nextSchedule.First.Id);
            _database.AssignUser(caseData.Id, nextSchedule.Second.Id);
            _database.AssignUser(caseData.Id, nextSchedule.Third.Id);
            nextSchedule.LastUsed = DateTime.Now;
            _database.UpdateSchedule(nextSchedule);
        }

        private static int GetMaxUsersAssignedToCase(Role role)
        {
            if (role == Role.Admin || role == Role.None || role == Role.System) return 0;
            if (role == Role.CaseWorker) return 1;
            if (role == Role.PanelMember) return 3;
            if (role == Role.Inquirer || role == Role.Solicitor || role == Role.ThirdPartyReviewer) return int.MaxValue;
            throw new ArgumentException("unknown user role");
        }

        private static bool CanSeeAllCases(UserHeader user)
        {
            return user.Role == Role.Admin || user.Role == Role.CaseWorker;
        }

        private static bool IsUserAssignedToCase(UserHeader user, CaseData caseData)
        {
            return caseData.AssignedUsers.Any(item => item.Id == user.Id);
        }

        private UserData GetUserByResetToken(ResetToken token)
        {
            if (token == null) return null;
            if (token.ExpirationTime < DateTime.Now) return null;
            var user = _database.GetUserById(token.UserId);
            return user;
        }

        private UserData GetUserByResetToken(string token)
        {
            if (token == null) return null;
            var tokenEntity = _database.GetResetToken(token);
            return GetUserByResetToken(tokenEntity);
        }

        private bool GetIsLooseUser(Guid id)
        {
            return _database.GetIsLooseUser(id);
        }

        #endregion

        #region Exceptions

        public class WeakSecurePhraseException : Exception
        {
        }

        public class WeakPasswordException : Exception
        {
        }

        public class UserDoesNotExistException : Exception
        {
        }

        public class SecurityTokenNotFoundException : Exception
        {
        }

        public class NotAuthorizedException : Exception
        {
        }

        public class NotAuthenticatedException : Exception
        {
        }

        public class InvalidUserOrPasswordException : Exception
        {
        }

        public class InvalidResetTokenException : Exception
        {
        }

        public class InvalidSecurePhraseAnswerException : Exception
        {
        }

        public class DuplicateUserException : Exception
        {
        }

        public class DuplicateCaseIdException : Exception
        {
        }

        public class CaseDoesNotExistsException : Exception
        {
        }

        public class OverAssignmentException : Exception
        {
            public OverAssignmentException(int caseId, Role role, UserHeader[] alreadyAssigned)
            {
                CaseId = caseId;
                Role = role;
                AlreadyAssigned = alreadyAssigned;
            }

            public int CaseId { get; set; }
            public Role Role { get; set; }
            public UserHeader[] AlreadyAssigned { get; set; }
        }

        public class ForbiddenException : Exception
        {
        }

        #endregion

        public ComplaintComments GetComments(UserHeader user, int caseId)
        {
            var @case = _database.GetCaseById(caseId);
            var status = GetCaseState(@case);

            var users = GetUsers().Select(x => new Contract.User
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = (Contract.Role) x.Role,
            }).ToArray();

            var allegations = _database.GetAllegationComments(caseId, user.Id);
            var preliminaryDecisionComments = _database.GetPreliminaryDecisionComments(caseId);
            var finalDecisionComments = _database.GetFinalDecisionComments(caseId);

            var finalDecisionApprovedByCurrentUser =
                finalDecisionComments.Any(x => x.PanelMemberId == user.Id && x.Decision == FinalDecisionCommentKind.Accept);

            var allegationsWithOthersComments = allegations
                .Select(x => new ComplaintComments.AllegationWithComments
                {
                    Id = x.Allegation.Id,
                    Text = x.Allegation.Text,
                    Comments = x.Comments.Select(y => new ComplaintComments.AllegationComment
                    {
                        Id = y.Id,
                        Text = y.Text,
                        User = users.FirstOrDefault(u => u.Id == y.PanelMemberId),
                    }).ToArray(),
                    Documents =
                        x.Allegation.Documents.Select(d => new Contract.Cases.Document {Id = d.Id, Name = d.Name}).ToArray(),
                }).ToArray();

            var allegationWithMyComments = allegations.Where(x => x.Allegation.MyComment != null)
                .Select(x => new ComplaintComments.AllegationWithMyComment
                {
                    Id = x.Allegation.Id,
                    Text = x.Allegation.Text,
                    Documents =
                        x.Allegation.Documents.Select(d => new Contract.Cases.Document {Id = d.Id, Name = d.Name}).ToArray(),
                    MyComment = new Contract.Cases.MyAllegationComment
                    {
                        Id = x.Allegation.MyComment.Id,
                        AllegationCommentType =
                            (Contract.Cases.AllegationCommentType) x.Allegation.MyComment.AllegationCommentType,
                        Text = x.Allegation.MyComment.Text,
                        AdditionalText = x.Allegation.MyComment.AdditionalText,
                    },
                    OtherComments = x.Comments
                        .Where(c => c.PanelMemberId != user.Id)
                        .Select(c => new ComplaintComments.AllegationComment
                        {
                            Id = c.Id,
                            Text = c.Text,
                            User = users.FirstOrDefault(u => u.Id == c.PanelMemberId),
                        }).ToArray(),
                });

            var allegationWithNoComments = allegations.Where(x => x.Allegation.MyComment == null)
                .Select(x => new ComplaintComments.AllegationWithMyComment
                {
                    Id = x.Allegation.Id,
                    Text = x.Allegation.Text,
                    Documents =
                        x.Allegation.Documents.Select(d => new Contract.Cases.Document {Id = d.Id, Name = d.Name}).ToArray(),
                    MyComment = new Contract.Cases.MyAllegationComment
                    {
                        Id = Guid.Empty,
                        Text = null,
                        AdditionalText = null,
                        AllegationCommentType = Contract.Cases.AllegationCommentType.No,
                    },
                    OtherComments = x.Comments
                        .Where(c => c.PanelMemberId != user.Id)
                        .Select(c => new ComplaintComments.AllegationComment
                        {
                            Id = c.Id,
                            Text = c.Text,
                            User = users.FirstOrDefault(u => u.Id == c.PanelMemberId),
                        }).ToArray(),
                });
            var partiesComments = _database.GetPartiesComments(caseId);
            var result = new ComplaintComments
            {
                DueDate = status.DueByDate,
                DueDaysLeft = status.DueDaysLeft,
                Status = GetStatus(status),
                FinalDecisionApprovedByCurrentUser = finalDecisionApprovedByCurrentUser,
                PreliminaryComments = new ComplaintComments.PreliminaryCommentsData
                {
                    AllegationsWithComments = allegationsWithOthersComments,
                    AllegationsWithMyComments = allegationWithMyComments.Concat(allegationWithNoComments).ToArray(),
                },
                PreliminaryDecisionComments = new ComplaintComments.PreliminaryDecisionCommentsData
                {
                    Comments = preliminaryDecisionComments.Select(x => new ComplaintComments.AllegationComment
                    {
                        Text = x.Text,
                        User = users.FirstOrDefault(u => u.Id == user.Id),
                    }).ToArray(),
                    CommentsFromParies = partiesComments.Select(x => new ComplaintComments.CommentFromParties
                    {
                        Text = x.Text,
                        Documents = x.Documents.Select(d => new Contract.Cases.Document {Id = d.Id, Name = d.Name}).ToArray(),
                    }).ToArray(),
                    PreliminaryDecisionDocument = @case.PreliminaryDecisionDocument == null
                        ? null
                        : new Contract.Cases.Document
                        {
                            Id = @case.PreliminaryDecisionDocument.Id,
                            Name = @case.PreliminaryDecisionDocument.Name,
                        },
                },
                FinalDecisionDocument = @case.FinalDecisionDocument == null
                    ? null
                    : new Contract.Cases.Document
                    {
                        Id = @case.FinalDecisionDocument.Id,
                        Name = @case.FinalDecisionDocument.Name,
                    },
            };

            return result;
        }

        private ComplaintComments.StatusKind GetStatus(CaseState status)
        {
            if (status.StateKind == CaseStateKind.New || status.StateKind == CaseStateKind.PriliminaryComments)
                return ComplaintComments.StatusKind.PreliminaryComments;
            if (status.StateKind == CaseStateKind.PriliminaryDecision)
                return ComplaintComments.StatusKind.PreliminaryDecisionComments;
            if (status.StateKind == CaseStateKind.WaitingForPartyComments)
                return ComplaintComments.StatusKind.PreliminaryDecisionWaiting;
            if (status.StateKind == CaseStateKind.FinalDecision)
                return ComplaintComments.StatusKind.FinalDecisionComments;
            if (status.StateKind == CaseStateKind.Locked)
                return ComplaintComments.StatusKind.FinalDecisionLocked;
            return ComplaintComments.StatusKind.Complete;
        }

        public Post[] GetPosts(int caseId)
        {
            return _database.GetPosts(caseId);
        }

        public void CreatePost(Guid postId, int caseId, Guid userId, string text)
        {
            _database.CreatePost(postId, caseId, DateTime.Now, userId, text);
        }

        public void CreateReply(Guid postId, int caseId, Guid replyOnId, Guid userId, string text)
        {
            _database.ReplyPost(postId, caseId, replyOnId, DateTime.Now, userId, text);
        }

        public void CreateMessage(Guid id, Guid fromUserId, Guid toUserId, string message,
            CreateNotification.DeliveryType delivery)
        {
            if (delivery == CreateNotification.DeliveryType.Both || delivery == CreateNotification.DeliveryType.System)
            {
                _database.CreateMessage(new Message
                {
                    Id = id,
                    FromUserId = fromUserId,
                    ToUserId = toUserId,
                    Text = message,
                    Created = DateTime.Now,
                    Accepted = null,
                });
            }
            if (delivery == CreateNotification.DeliveryType.Both || delivery == CreateNotification.DeliveryType.Email)
            {
                // TODO: mail notification
            }
        }

        public MessageEx[] GetMessages(Guid toUserId)
        {
            var messages = _database.GetNotAcceptedMessagesByToUserId(toUserId);
            return messages;
        }

        public void AcceptMessage(Guid userId, Guid messageId)
        {
            var message = _database.GetMessageById(messageId);
            if (message.ToUserId != userId) throw new ForbiddenException();
            _database.SetMessageAcceptedDate(messageId, DateTime.Now);
        }

        public NewDocument GetDocument(Guid documentId)
        {
            var result = new NewDocument
            {
                Id = documentId,
                Name = _database.GetDocumentById(documentId).Name,
                Body = _documentStore.Download(documentId).Content,
            };
            return result;
        }

        public void AddPartiesComment(UserHeader user, NewPartiesComment newPartiesComment)
        {
            _database.AddPartyComment(newPartiesComment, DateTime.Now);
            foreach (var document in newPartiesComment.Documents)
            {
                _documentStore.Upload(document.Id, newPartiesComment.CaseId, new DocumentStoreItem
                {
                    Name = document.Name,
                    Content = document.Body,
                });
                _database.AddDocumentActivity(new DocumentActivity
                {
                    Id = Guid.NewGuid(),
                    CaseId = newPartiesComment.CaseId,
                    DocumentId = document.Id,
                    DocumentName = document.Name,
                    DocumentType = DocumentType.PartyComment,
                    ActionType = ActionType.Create,
                    UserId = user.Id,
                    Date = DateTime.Now
                });
            }
        }

        public void AddPreliminaryDecisionDocument(UserHeader user, int caseId, NewDocument document)
        {
            var doc = _database.GetPreliminaryDecisionDocument(caseId);
            _database.AddPreliminaryDecisionDocument(caseId, document.Id, document.Name);
            if (doc != null)
            {
               _documentStore.Update(doc.Id, caseId, new DocumentStoreItem {Name = doc.Name, Content = document.Body,});
                _database.AddDocumentActivity(new DocumentActivity
                {
                    Id = Guid.NewGuid(),
                    CaseId = caseId,
                    DocumentId = document.Id,
                    DocumentName = document.Name,
                    DocumentType = DocumentType.PreliminaryDecision,
                    ActionType = ActionType.Create,
                    UserId = user.Id,
                    Date = DateTime.Now
                });
            }
            else
            {
                _documentStore.Upload(document.Id, caseId, new DocumentStoreItem {Name = document.Name, Content = document.Body,});
                _database.AddDocumentActivity(new DocumentActivity
                {
                    Id = Guid.NewGuid(),
                    CaseId = caseId,
                    DocumentId = document.Id,
                    DocumentName = document.Name,
                    DocumentType = DocumentType.PreliminaryDecision,
                    ActionType = ActionType.Create,
                    UserId = user.Id,
                    Date = DateTime.Now
                });
            }
        }

        public void AddFinalDecisionDocument(UserHeader user, int caseId, NewDocument document)
        {
            _database.AddFinalDecisionDocument(caseId, document.Id, document.Name);
            _documentStore.Upload(document.Id, caseId, new DocumentStoreItem {Name = document.Name, Content = document.Body,});
            _database.AddDocumentActivity(new DocumentActivity
            {
                Id = Guid.NewGuid(),
                CaseId = caseId,
                DocumentId = document.Id,
                DocumentName = document.Name,
                DocumentType = DocumentType.FinalDecision,
                ActionType = ActionType.Create,
                UserId = user.Id,
                Date = DateTime.Now
            });
        }

        public void UpdateFinalDecisionDocument(UserHeader user, int caseId, NewDocument document)
        {
            var existingDocument = _database.GetFinalDecisionDocument(caseId);
            if (existingDocument == null)
            {
                document.Id = Guid.NewGuid();
                AddFinalDecisionDocument(user, caseId, document);
            }
            else
            {
                _documentStore.Update(existingDocument.Id, caseId, new DocumentStoreItem {Name = existingDocument.Name, Content = document.Body,});
                _database.AddDocumentActivity(new DocumentActivity
                {
                    Id = Guid.NewGuid(),
                    CaseId = caseId,
                    DocumentId = document.Id,
                    DocumentName = document.Name,
                    DocumentType = DocumentType.FinalDecision,
                    ActionType = ActionType.Update,
                    UserId = user.Id,
                    Date = DateTime.Now
                });
            }
        }

        //newly added on sep 4th
        public void UpdatePreliminaryDecisionDocument(UserHeader user, int caseId, NewDocument document)
        {
            var existingDocument = _database.GetPreliminaryDecisionDocument(caseId);
            if (existingDocument == null)
            {
                document.Id = Guid.NewGuid();
                AddPreliminaryDecisionDocument(user, caseId, document);                
            }
            else
            {
                _documentStore.Update(existingDocument.Id, caseId, new DocumentStoreItem {Name = existingDocument.Name, Content = document.Body,});
                _database.AddDocumentActivity(new DocumentActivity
                {
                    Id = Guid.NewGuid(),
                    CaseId = caseId,
                    DocumentId = document.Id,
                    DocumentName = document.Name,
                    DocumentType = DocumentType.PreliminaryDecision,
                    ActionType = ActionType.Update,
                    UserId = user.Id,
                    Date = DateTime.Now
                });
            }
        }


        public Document[] GetAllDocuments(int caseId)
        {
            return _database.GetAllDocuments(caseId);
        }

        #region Test

        public UserHeader CreateDefaultUser(string password, string phrase)
        {
            const string defaultUserEmail = "admin@arb.org.uk";
            var defaultUserId = Guid.Empty;

            var salt = _passwordManager.GenerateSalt();
            var hashedPassword = _passwordManager.HashPassword(password, salt);
            var enctyptedSecurePhrase = _passwordManager.EncryptSecurePhrase(phrase);

            var existingUser = _database.GetUserById(defaultUserId);
            if (existingUser == null)
            {
                var user = new UserData
                {
                    Id = defaultUserId,
                    Email = defaultUserEmail,
                    FirstName = "Default",
                    LastName = "User",
                    Role = Role.Admin,
                    UserState = UserState.Activated,
                    HashedPassword = hashedPassword,
                    PasswordSalt = salt,
                    EncryptedSecurePhrase = enctyptedSecurePhrase,
                    FirstSecurePhraseQuestionCharacterIndex = 0,
                    SecondSecurePhraseQuestionCharacterIndex = 1
                };
                _database.CreateUser(user);
                return user;
            }
            else
            {
                existingUser.HashedPassword = hashedPassword;
                existingUser.PasswordSalt = salt;
                existingUser.EncryptedSecurePhrase = enctyptedSecurePhrase;
                existingUser.FirstSecurePhraseQuestionCharacterIndex = 0;
                existingUser.SecondSecurePhraseQuestionCharacterIndex = 1;
                existingUser.UserState = UserState.Activated;
                _database.UpdateUser(existingUser);
                return existingUser;
            }
        }

        public void SetCaseStartDate(int id, DateTime date)
        {
            _database.SetCaseStartDate(id, date);
        }

        public UserHeader CreateActiveUser(string id, string password, string phrase, Role role)
        {
            var email = id + "@arb.org.uk";
            var salt = _passwordManager.GenerateSalt();
            var hashedPassword = _passwordManager.HashPassword(password, salt);
            var enctyptedSecurePhrase = _passwordManager.EncryptSecurePhrase(phrase);

            var existingUser = _database.GetUserByEmail(email);
            if (existingUser == null)
            {
                var user = new UserData
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    FirstName = "User",
                    LastName = id,
                    Role = role,
                    UserState = UserState.Activated,
                    HashedPassword = hashedPassword,
                    PasswordSalt = salt,
                    EncryptedSecurePhrase = enctyptedSecurePhrase,
                    FirstSecurePhraseQuestionCharacterIndex = 0,
                    SecondSecurePhraseQuestionCharacterIndex = 1
                };
                _database.CreateUser(user);
                return user;
            }
            else
            {
                existingUser.HashedPassword = hashedPassword;
                existingUser.PasswordSalt = salt;
                existingUser.EncryptedSecurePhrase = enctyptedSecurePhrase;
                existingUser.FirstSecurePhraseQuestionCharacterIndex = 0;
                existingUser.SecondSecurePhraseQuestionCharacterIndex = 1;
                existingUser.UserState = UserState.Activated;
                _database.UpdateUser(existingUser);
                return existingUser;
            }
        }

        #endregion

        public void NotifyCaseWorkersAboutClosingCases()
        {
            ;
        }

        //prabhakar
        public List<UserHeader> GetCaseWorkerDetails(int caseId)
        {
            UserHeader[] userdetails = _database.GetAssignedUsers(caseId);
            var user = from item in userdetails
                       where item.Role == Role.CaseWorker
                       select item;
            return user.ToList();
        }
    }
}