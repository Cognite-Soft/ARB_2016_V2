using System;
using System.Linq;
using System.Data.Entity;
using Cognite.Arb.Server.Business;
using Cognite.Arb.Server.Business.Cases;
using Cognite.Arb.Server.Business.Database;

namespace Cognite.Arb.Server.Resource.Database
{
    public class Database : IDatabase
    {
        public UserData GetUserById(Guid id)
        {
            using (var context = new DatabaseContext())
            {
                var result = context.Users.FirstOrDefault(x => x.Id == id);
                return result;
            }
        }

        public UserData GetUserByEmail(string email)
        {
            using (var context = new DatabaseContext())
            {
                var result = context.Users.FirstOrDefault(x => x.Email == email);
                return result;
            }
        }

        public UserHeader[] GetActiveUsers()
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Users.Where(x => x.UserState != UserState.Deleted).Select(x => new UserHeader
                {
                    Id = x.Id,
                    Email = x.Email,
                    Role = x.Role,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                });

                var result = query.ToArray();

                return result;
            }
        }

        public UserHeader[] GetAllUsers()
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Users.Select(x => new UserHeader
                {
                    Id = x.Id,
                    Email = x.Email,
                    Role = x.Role,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                });

                var result = query.ToArray();

                return result;
            }
        }

        public void CreateUser(UserData user)
        {
            using (var context = new DatabaseContext())
            {
                var entity = new UserEntity(user);
                context.Users.Add(entity);
                context.SaveChanges();
            }
        }

        public void UpdateUser(UserData user)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Users.FirstOrDefault(x => x.Email == user.Email);
                entity.Update(user);
                context.SaveChanges();
            }
        }

        public void DeleteUser(Guid id)
        {
            using (var context = new DatabaseContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Id == id);
                if (user.ResetTokens != null)
                    context.ResetTokens.RemoveRange(user.ResetTokens);
                if (user.AssignedCases != null)
                    user.AssignedCases.Clear();
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        public void AddResetToken(ResetToken token)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.ResetTokens.Create();
                entity.UserId = token.UserId;
                entity.Token = token.Token;
                entity.ExpirationTime = token.ExpirationTime;
                entity.Type = token.Type;
                context.ResetTokens.Add(entity);
                context.SaveChanges();
            }
        }

        public ResetToken GetResetToken(string token)
        {
            using (var context = new DatabaseContext())
            {
                var result = context.ResetTokens.FirstOrDefault(item => item.Token == token);
                return result;
            }
        }

        public void DeleteResetTokenIfExists(Guid userId, ResetTokenType type)
        {
            using (var context = new DatabaseContext())
            {
                var toDelete = context.ResetTokens.Where(item => item.UserId == userId && item.Type == type).ToArray();
                if (toDelete.Length == 0) return;
                context.ResetTokens.RemoveRange(toDelete);
                context.SaveChanges();
            }
        }

        public void DeleteExpiredResetTokens()
        {
            using (var context = new DatabaseContext())
            {
                var now = DateTime.Now;
                var query = context.ResetTokens.Where(x => x.ExpirationTime < now);
                context.ResetTokens.RemoveRange(query);
                context.SaveChanges();
            }
        }

        public CaseData GetCaseById(int id)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Cases.
                    Include(item => item.DatesAndDetails).
                    Include(item => item.Allegations).
                    Include(item => item.AssignedUsers).
                    FirstOrDefault(item => item.Id == id);
                if (entity == null) return null;

                var result = new CaseData
                {
                    Id = entity.Id,
                    ParentId = entity.ParentId,
                    Status = (CaseData.CaseStatus) entity.Status,
                    StartDate = entity.StartDate,
                    Background = entity.Background,
                    IdealOutcome = entity.IdealOutcome,
                    IssueRaisedWithArchitect = new CaseData.Question
                    {
                        Answer = entity.IssueRaisedWithArchitect.Answer,
                        Comments = entity.IssueRaisedWithArchitect.Comments,
                    },
                    SubjectOfLegalProceedings = new CaseData.Question
                    {
                        Answer = entity.SubjectOfLegalProceedings.Answer,
                        Comments = entity.SubjectOfLegalProceedings.Comments,
                    },
                    Relationship = entity.Relationship,
                    ContactAgreement = entity.ContactAgreement,
                    ClaimantContact = new CaseData.ContactData
                    {
                        Name = entity.ClaimantContact.Name,
                        EMail = entity.ClaimantContact.EMail,
                        Address = entity.ClaimantContact.Address,
                        Phone = entity.ClaimantContact.Phone,
                    },
                    ArchitectContact = new CaseData.ArchitectContactData
                    {
                        Name = entity.ArchitectContact.Name,
                        RegistrationNumber = entity.ArchitectContact.RegistrationNumber,
                        EMail = entity.ArchitectContact.EMail,
                        Address = entity.ArchitectContact.Address,
                        Phone = entity.ArchitectContact.Phone,
                    },
                    AssignedUsers = entity.AssignedUsers.Select(item => new UserHeader
                    {
                        Id = item.Id,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Role = item.Role,
                    }).ToArray(),
                    DatesAndDetails = entity.DatesAndDetails.OrderBy(x => x.Date).ThenBy(x => x.Order)
                        .Select(item => new CaseData.DatesAndDetail
                        {
                            Id = item.Id,
                            Date = item.Date,
                            Text = item.Text,
                            Documents = item.Documents.Select(d => new CaseData.Document {Id = d.Id, Name = d.Name}),
                        }).ToArray(),
                    Allegations = entity.Allegations
                        .OrderBy(item => item.Authorship.Date).ThenBy(item => item.Order)
                        .Select(item => new CaseData.Allegation
                        {
                            Id = item.Id,
                            Text = item.Text,
                            Documents = item.Documents.Select(d => new CaseData.Document {Id = d.Id, Name = d.Name}).ToArray(),
                            Comments = item.Comments.OrderByDescending(c => c.Authorship.Date).Select(c => new CaseData.Comment
                            {
                                CommentType = (CaseData.CommentType) c.Type,
                                Text = c.Text,
                                AdditionalText = c.AdditionalText,
                                PanelMemberId = c.Authorship.UserId,
                            }).ToArray(),
                        }).ToArray(),
                    ProcessStartDate = entity.ProcessStartDate,
                };

                result.PreliminaryDecisionDocument = entity.PreliminaryDecisionDocument == null
                    ? null
                    : new CaseData.Document
                    {
                        Id = entity.PreliminaryDecisionDocument.Id,
                        Name = entity.PreliminaryDecisionDocument.Name
                    };
                result.FinalDecisionDocument = entity.FinalDecisionDocument == null
                    ? null
                    : new CaseData.Document
                    {
                        Id = entity.FinalDecisionDocument.Id,
                        Name = entity.FinalDecisionDocument.Name
                    };

                return result;
            }
        }

        public void CreateCase(CaseData caseData)
        {
            using (var context = new DatabaseContext())
            {
                var newCase = new CaseEntity
                {
                    Id = caseData.Id,
                    ParentId = caseData.ParentId,
                    Status = (CaseEntity.CaseStatus) caseData.Status,
                    StartDate = caseData.StartDate,
                    Background = caseData.Background,
                    IdealOutcome = caseData.IdealOutcome,
                    IssueRaisedWithArchitect = new CaseEntity.Question
                    {
                        Answer = caseData.IssueRaisedWithArchitect.Answer,
                        Comments = caseData.IssueRaisedWithArchitect.Comments,
                    },
                    SubjectOfLegalProceedings = new CaseEntity.Question
                    {
                        Answer = caseData.SubjectOfLegalProceedings.Answer,
                        Comments = caseData.SubjectOfLegalProceedings.Comments,
                    },
                    Relationship = caseData.Relationship,
                    ContactAgreement = caseData.ContactAgreement,
                    ClaimantContact = new CaseEntity.Contact
                    {
                        Name = caseData.ClaimantContact.Name,
                        EMail = caseData.ClaimantContact.EMail,
                        Address = caseData.ClaimantContact.Address,
                        Phone = caseData.ClaimantContact.Phone,
                    },
                    ArchitectContact = new CaseEntity.Contact
                    {
                        Name = caseData.ArchitectContact.Name,
                        RegistrationNumber = caseData.ArchitectContact.RegistrationNumber,
                        EMail = caseData.ArchitectContact.EMail,
                        Address = caseData.ArchitectContact.Address,
                        Phone = caseData.ArchitectContact.Phone,
                    },
                };
                context.Cases.Add(newCase);
                context.SaveChanges();
            }
        }

        public void UpdateCase(CaseData caseData)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Cases.FirstOrDefault(c => c.Id == caseData.Id);
                if (entity == null) throw new Facade.CaseDoesNotExistsException();

                entity.Status = (CaseEntity.CaseStatus) caseData.Status;
                entity.StartDate = caseData.StartDate;
                entity.Background = caseData.Background;
                entity.IdealOutcome = caseData.IdealOutcome;
                entity.IssueRaisedWithArchitect = new CaseEntity.Question
                {
                    Answer = caseData.IssueRaisedWithArchitect.Answer,
                    Comments = caseData.IssueRaisedWithArchitect.Comments,
                };
                entity.SubjectOfLegalProceedings = new CaseEntity.Question
                {
                    Answer = caseData.SubjectOfLegalProceedings.Answer,
                    Comments = caseData.SubjectOfLegalProceedings.Comments,
                };
                entity.Relationship = caseData.Relationship;
                entity.ContactAgreement = caseData.ContactAgreement;
                entity.ClaimantContact = new CaseEntity.Contact
                {
                    Address = caseData.ClaimantContact.Address,
                    EMail = caseData.ClaimantContact.EMail,
                    Name = caseData.ClaimantContact.Name,
                    Phone = caseData.ClaimantContact.Phone,
                };
                entity.ArchitectContact = new CaseEntity.Contact
                {
                    Address = caseData.ArchitectContact.Address,
                    EMail = caseData.ArchitectContact.EMail,
                    Name = caseData.ArchitectContact.Name,
                    Phone = caseData.ArchitectContact.Phone,
                    RegistrationNumber = caseData.ArchitectContact.RegistrationNumber,
                };
                entity.ProcessStartDate = caseData.ProcessStartDate;

                context.SaveChanges();
            }
        }

        public CaseHeader[] GetAllCaseHeaders()
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Cases
                    .Select(item => new CaseHeader
                    {
                        Id = item.Id,
                        Complainant = item.ClaimantContact.Name,
                        Architect = item.ArchitectContact.Name,
                        RegistrationNumber = item.ArchitectContact.RegistrationNumber,
                        Status = (CaseData.CaseStatus) item.Status,
                    });
                var result = query.ToArray();
                return result;
            }
        }

        public CaseHeader[] GetCaseHeadersByAssignedUserId(Guid id)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Cases
                    .Where(item => item.AssignedUsers.Any(user => user.Id == id))
                    .Select(item => new CaseHeader
                    {
                        Id = item.Id,
                        Complainant = item.ClaimantContact.Name,
                        Architect = item.ArchitectContact.Name,
                        RegistrationNumber = item.ArchitectContact.RegistrationNumber,
                        Status = (CaseData.CaseStatus) item.Status,
                    });
                var result = query.ToArray();
                return result;
            }
        }

        public UserHeader[] GetAssignedUsers(int caseId)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Cases
                    .Include(item => item.AssignedUsers)
                    .Where(item => item.Id == caseId)
                    .SelectMany(item => item.AssignedUsers)
                    .Select(item => new UserHeader
                    {
                        Id = item.Id,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Role = item.Role,
                    })
                    .Distinct();
                var result = query.ToArray();
                return result;
            }
        }

        public void AssignUser(int caseId, Guid userId)
        {
            using (var context = new DatabaseContext())
            {
                var @case = context.Cases.FirstOrDefault(item => item.Id == caseId);
                if (@case == null) throw new Facade.CaseDoesNotExistsException();
                var user = context.Users.FirstOrDefault(item => item.Id == userId);
                if (user == null) throw new Facade.UserDoesNotExistException();
                @case.AssignedUsers.Add(user);
                context.SaveChanges();
            }
        }

        public bool DisassignUser(int caseId, Guid userId)
        {
            using (var context = new DatabaseContext())
            {
                var @case = context.Cases.Include(item => item.AssignedUsers).FirstOrDefault(item => item.Id == caseId);
                if (@case == null) throw new Facade.CaseDoesNotExistsException();
                var user = @case.AssignedUsers.FirstOrDefault(item => item.Id == userId);
                if (user == null) return false;
                @case.AssignedUsers.Remove(user);
                context.SaveChanges();
                return true;
            }
        }

        public int[] GetUserAssignments(Guid id)
        {
            using (var context = new DatabaseContext())
            {
                var user = context.Users.Include(item => item.AssignedCases).FirstOrDefault(item => item.Id == id);
                if (user == null) throw new Facade.UserDoesNotExistException();
                return user.AssignedCases.Select(item => item.Id).ToArray();
            }
        }

        public Schedule[] GetSchedules()
        {
            using (var context = new DatabaseContext())
            {
                var existingSchedules = SelectSchedules(context);
                if (existingSchedules.Length != 0)
                    return existingSchedules;
                var newSchedules = CreateSchedules();
                SaveNewSchedules(newSchedules, context);
                return newSchedules;
            }
        }

        public Schedule GetSchedule(int row)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Schedules.FirstOrDefault(item => item.Id == row);
                if (entity == null) return null;
                return new Schedule
                {
                    Id = entity.Id,
                    LastUsed = entity.LastUsed,
                    First = entity.User1,
                    Second = entity.User2,
                    Third = entity.User3,
                };
            }
        }

        public void UpdateSchedule(Schedule schedule)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Schedules.FirstOrDefault(item => item.Id == schedule.Id);
                entity.User1Id = schedule.First == null ? (Guid?) null : schedule.First.Id;
                entity.User2Id = schedule.Second == null ? (Guid?) null : schedule.Second.Id;
                entity.User3Id = schedule.Third == null ? (Guid?) null : schedule.Third.Id;
                context.SaveChanges();
            }
        }

        public int GetMaxDateAndDetailOrder(int caseId, DateTime date)
        {
            using (var context = new DatabaseContext())
            {
                var orders =
                    context.DatesAndDetails.Where(x => x.CaseId == caseId && x.Date == date).Select(x => x.Order).ToArray();
                if (orders.Length == 0) return 0;
                return orders.Max();
            }
        }

        public void CreateDateAndDetail(int caseId, NewDateAndDetail item, Guid userId, int order)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.DatesAndDetails.Create();
                entity.Id = item.Id;
                entity.CaseId = caseId;
                entity.Date = item.Date;
                entity.Text = item.Text;
                entity.Order = order;
                entity.Authorship = new Authorship
                {
                    Date = DateTime.Now,
                    UserId = userId,
                };
                foreach (var document in item.Documents)
                {
                    var documentEntity = context.Documents.Create();
                    documentEntity.Id = document.Id;
                    documentEntity.Name = document.Name;
                    context.Documents.Add(documentEntity);
                    entity.Documents.Add(documentEntity);
                }

                context.DatesAndDetails.Add(entity);
                context.SaveChanges();
            }
        }

        public void DeleteAllegation(Guid id)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Allegations.FirstOrDefault(x => x.Id == id);
                if (entity == null) throw new Exception();
                entity.Documents.Clear();
                context.Allegations.Remove(entity);
                context.SaveChanges();
            }
        }

        public void CreateAllegation(int caseId, NewAllegation item, Guid userId, int order)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Allegations.Create();
                entity.Id = item.Id;
                entity.CaseId = caseId;
                entity.Text = item.Text;
                entity.Order = order;
                entity.Authorship = new Authorship
                {
                    Date = DateTime.Now,
                    UserId = userId,
                };
                foreach (var document in item.Documents)
                {
                    var documentEntity = context.Documents.Create();
                    documentEntity.Id = document.Id;
                    documentEntity.Name = document.Name;
                    context.Documents.Add(documentEntity);
                    entity.Documents.Add(documentEntity);
                }
                context.Allegations.Add(entity);
                context.SaveChanges();
            }
        }

        public int? GetCaseIdByAllegationId(Guid allegationId)
        {
            using (var context = new DatabaseContext())
            {
                var result = context.Allegations.Where(a => a.Id == allegationId).Select(a => a.CaseId).ToArray();
                if (result.Length == 0) return null;
                return result[0];
            }
        }

        public void CreateAllegationComment(Guid allegationId, string comment, AllegationCommentType type,
            string additionalComment, Guid userId)
        {
            if (additionalComment == string.Empty) additionalComment = null;
            using (var context = new DatabaseContext())
            {
                var allegation = context.Allegations.FirstOrDefault(a => a.Id == allegationId);
                if (allegation == null) throw new Exception();
                var allegationCommentEntity = new AllegationCommentEntity
                {
                    Id = Guid.NewGuid(),
                    AllegationId = allegationId,
                    Text = comment,
                    AdditionalText = additionalComment,
                    Type = type,
                    Authorship = new Authorship
                    {
                        Date = DateTime.Now,
                        UserId = userId,
                    }
                };
                allegation.Comments.Add(allegationCommentEntity);
                context.SaveChanges();
            }
        }

        public ActivityFeed.ActivityFeedItem[] GetPreliminaryCommentsActivityFeedItems(int caseId)
        {
            var users = GetActiveUsers();
            using (var context = new DatabaseContext())
            {
                var allegationItems =
                    context.Allegations.Where(x => x.CaseId == caseId)
                        .OrderBy(x => x.Order)
                        .SelectMany(a => a.Comments)
                        .Select(c => new
                        {
                            c.Id,
                            c.Authorship.Date,
                            c.Authorship.UserId,
                            c.Type,
                            c.Text,
                            c.AdditionalText,
                        }).ToArray();
                var allegationFeedItems = allegationItems.Select(item => new ActivityFeed.ActivityFeedItem
                {
                    Action = ActivityFeed.ActivityFeedAction.PreliminaryAllegationComment,
                    Id = item.Id,
                    User = users.FirstOrDefault(u => u.Id == item.UserId),
                    Date = item.Date,
                    Description = GetAllegationActivityFeedItemDescription(item.Type, item.Text, item.AdditionalText),
                }).ToArray();

                return allegationFeedItems;
            }
        }

        public ActivityFeed.ActivityFeedItem[] GetPreliminaryDecisionActivityFeedItems(int caseId)
        {
            var users = GetActiveUsers();
            using (var context = new DatabaseContext())
            {
                var comments = context.PreliminaryDecisionComments.Where(x => x.CaseId == caseId).OrderBy(x => x.Date).ToArray();
                var result = comments.Select(x =>
                    new ActivityFeed.ActivityFeedItem
                    {
                        Action = ActivityFeed.ActivityFeedAction.PreliminaryDecisionComment,
                        Id = x.Id,
                        User = users.FirstOrDefault(u => u.Id == x.PanelMemberId),
                        Description = x.Text,
                        Date = x.Date,
                    }).ToArray();
                return result;
            }
        }

        public ActivityFeed.ActivityFeedItem[] GetFinalDecisionActivityFeedItems(int caseId)
        {
            var users = GetActiveUsers();
            using (var context = new DatabaseContext())
            {
                var comments = context.FinalDecisionComments.Where(x => x.CaseId == caseId).OrderBy(x => x.Date).ToArray();
                var result = comments.Select(x =>
                    new ActivityFeed.ActivityFeedItem
                    {
                        Action = ActivityFeed.ActivityFeedAction.PreliminaryDecisionComment,
                        Id = x.Id,
                        User = users.FirstOrDefault(u => u.Id == x.PanelMemberId),
                        Description = GetFinalDecisionActivityFeedItemDescription(x.Decision, x.CommentForChange),
                        Date = x.Date,
                    }).ToArray();
                return result;
            }
        }

        private static string GetFinalDecisionActivityFeedItemDescription(FinalDecisionCommentKind decision,
            string commentForChange)
        {
            switch (decision)
            {
                case FinalDecisionCommentKind.Accept:
                    return "Accept";
                case FinalDecisionCommentKind.Ammend:
                    return "Amend";
                case FinalDecisionCommentKind.Changed:
                    return "Preliminary Decision Changed: " + commentForChange;
                default:
                    throw new ArgumentException();
            }
        }

        public void CreatePreliminaryDecisionComment(PreliminaryDecisionComment comment)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.PreliminaryDecisionComments.Create();
                entity.Id = comment.Id;
                entity.CaseId = comment.CaseId;
                entity.PanelMemberId = comment.PanelMemberId;
                entity.Text = comment.Text;
                entity.Date = comment.Date;
                context.PreliminaryDecisionComments.Add(entity);
                context.SaveChanges();
            }
        }

        public void CreateFinalDecisionComment(FinalDecisionComment comment)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.FinalDecisionComments.Create();
                entity.Id = comment.Id;
                entity.CaseId = comment.CaseId;
                entity.PanelMemberId = comment.PanelMemberId;
                entity.Decision = comment.Decision;
                entity.CommentForChange = comment.CommentForChange;
                entity.Date = comment.Date;
                context.FinalDecisionComments.Add(entity);
                context.SaveChanges();
            }
        }

        public PreliminaryDecisionComment[] GetPreliminaryDecisionComments(int caseId)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.PreliminaryDecisionComments.Where(x => x.CaseId == caseId).OrderBy(x => x.Date);
                var result = query.ToArray().Cast<PreliminaryDecisionComment>().ToArray();
                return result;
            }
        }

        public FinalDecisionComment[] GetFinalDecisionComments(int caseId)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.FinalDecisionComments.Where(x => x.CaseId == caseId).OrderBy(x => x.Date);
                var result = query.ToArray().Cast<FinalDecisionComment>().ToArray();
                return result;
            }
        }

        public AllegationEx[] GetAllegationComments(int caseId, Guid userId)
        {
            using (var context = new DatabaseContext())
            {
                var allegationEntities = context.Allegations.Include(x => x.Comments).Where(x => x.CaseId == caseId)
                    .OrderBy(item => item.Authorship.Date).ThenBy(item => item.Order)
                    .ToArray();

                var result = allegationEntities.Select(x => new AllegationEx
                {
                    Allegation = new Allegation
                    {
                        Id = x.Id,
                        CanBeDeleted = false,
                        Text = x.Text,
                        Documents = x.Documents.Select(d => new Document {Id = d.Id, Name = d.Name}).ToArray(),
                        MyComment = x.Comments.Where(c => c.Authorship.UserId == userId).Select(c => new MyAllegationComment
                        {
                            AllegationCommentType = c.Type,
                            Id = c.Id,
                            Text = c.Text,
                            AdditionalText = c.AdditionalText,
                        }).FirstOrDefault(),
                    },
                    Comments = x.Comments.Select(c => new AllegationEx.AllegationComment
                    {
                        Id = c.Id,
                        Text = c.Text,
                        PanelMemberId = c.Authorship.UserId,
                    }).ToArray(),
                }).ToArray();

                return result;
            }
        }

        public void CreatePost(Guid postId, int caseId, DateTime date, Guid userId, string text)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Posts.Create();
                entity.Id = postId;
                entity.ParentId = null;
                entity.CaseId = caseId;
                entity.UserId = userId;
                entity.Text = text;
                entity.Date = date;
                context.Posts.Add(entity);
                context.SaveChanges();
            }
        }

        public void ReplyPost(Guid postId, int caseId, Guid replyOnId, DateTime date, Guid userId, string text)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Posts.Create();
                entity.Id = postId;
                entity.ParentId = replyOnId;
                entity.CaseId = caseId;
                entity.UserId = userId;
                entity.Text = text;
                entity.Date = date;
                context.Posts.Add(entity);
                context.SaveChanges();
            }
        }

        public Post[] GetPosts(int caseId)
        {
            using (var context = new DatabaseContext())
            {
                var users = context.Users.Select(x => new UserHeader
                {
                    Id = x.Id,
                    Email = x.Email,
                    Role = x.Role,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                }).ToArray();
                var entities = context.Posts.Where(x => x.CaseId == caseId).ToArray();
                var posts = entities.Select(x => new Post
                {
                    Id = x.Id,
                    Date = x.Date,
                    Text = x.Text,
                    User = users.FirstOrDefault(u => u.Id == x.UserId),
                    Parent = x.ParentId == null
                        ? null
                        : entities.Where(r => r.Id == x.ParentId).Select(r => new Reply
                        {
                            Id = r.Id,
                            Date = r.Date,
                            User = users.FirstOrDefault(u => u.Id == r.UserId),
                            Text = r.Text,
                        }).FirstOrDefault(),
                }).ToArray();
                //var posts = entities.Where(x => x.ParentId == null).Select(x => new Post
                //{
                //    Id = x.Id,
                //    Text = x.Text,
                //    User = users.FirstOrDefault(u => u.Id == x.UserId),
                //    Replies = entities.Where(r => r.ParentId == x.Id).Select(r => new Reply
                //    {
                //        Id = r.Id,
                //        User = users.FirstOrDefault(u => u.Id == r.UserId),
                //        Text = r.Text,
                //    }).ToArray(),
                //}).ToArray();
                return posts;
            }
        }

        public void CreateMessage(Message message)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Messages.Create();
                entity.Id = message.Id;
                entity.FromUserId = message.FromUserId;
                entity.ToUserId = message.ToUserId;
                entity.Text = message.Text;
                entity.Created = message.Created;
                entity.Accepted = message.Accepted;
                context.Messages.Add(entity);
                context.SaveChanges();
            }
        }

        public Message GetMessageById(Guid id)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Messages.Where(x => x.Id == id)
                    .Select(x => new Message
                    {
                        Id = x.Id,
                        FromUserId = x.FromUserId,
                        ToUserId = x.ToUserId,
                        Text = x.Text,
                        Accepted = x.Accepted,
                    });
                var message = query.FirstOrDefault();
                return message;
            }
        }

        public void SetMessageAcceptedDate(Guid messageId, DateTime accepted)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Messages.FirstOrDefault(x => x.Id == messageId);
                entity.Accepted = accepted;
                context.SaveChanges();
            }
        }

        public MessageEx[] GetNotAcceptedMessagesByToUserId(Guid toUserId)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Messages
                    .Include(x => x.FromUser)
                    .Where(x => x.Accepted == null && x.ToUserId == toUserId)
                    .Select(x => new MessageEx
                    {
                        Id = x.Id,
                        FromUserId = x.FromUserId,
                        ToUserId = x.ToUserId,
                        Text = x.Text,
                        Created = x.Created,
                        Accepted = x.Accepted,
                        FromUser = new UserHeader
                        {
                            Id = x.FromUser.Id,
                            Email = x.FromUser.Email,
                            Role = x.FromUser.Role,
                            FirstName = x.FromUser.FirstName,
                            LastName = x.FromUser.LastName,
                        },
                    });
                var messages = query.ToArray();
                return messages;
            }
        }

        public Document[] GetAllegationDocuments(Guid allegationId)
        {
            using (var context = new DatabaseContext())
            {
                var allegation = context.Allegations.FirstOrDefault(x => x.Id == allegationId);
                if (allegation == null) return new Document[0];
                var documents = allegation.Documents.Select(x => new Document {Id = x.Id, Name = x.Name}).ToArray();
                return documents;
            }
        }

        public void AddPartyComment(NewPartiesComment comment, DateTime now)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.PartiesComments.Create();
                entity.Id = comment.Id;
                entity.CaseId = comment.CaseId;
                entity.Comment = comment.Text;
                entity.Date = now;
                foreach (var document in comment.Documents)
                {
                    var documentEntity = context.Documents.Create();
                    documentEntity.Id = document.Id;
                    documentEntity.Name = document.Name;
                    context.Documents.Add(documentEntity);
                    entity.Documents.Add(documentEntity);
                }
                context.PartiesComments.Add(entity);
                context.SaveChanges();
            }
        }

        public PartiesComment[] GetPartiesComments(int caseId)
        {
            using (var context = new DatabaseContext())
            {
                var raw = context.PartiesComments
                    .Include(x => x.Documents)
                    .Where(x => x.CaseId == caseId)
                    .OrderBy(x => x.Date)
                    .Select(x => new
                    {
                        Text = x.Comment,
                        Documents = x.Documents.Select(d => new Document {Id = d.Id, Name = d.Name}),
                    }).ToArray();
                var result = raw.Select(x => new PartiesComment
                {
                    Text = x.Text,
                    Documents = x.Documents.ToArray(),
                }).ToArray();
                return result;
            }
        }

        public void AddPreliminaryDecisionDocument(int caseId, Guid id, string name)
        {
            using (var context = new DatabaseContext())
            {
                var @case = context.Cases.FirstOrDefault(x => x.Id == caseId);
                if (@case == null) throw new Facade.CaseDoesNotExistsException();

                if (@case.PreliminaryDecisionDocumentId != null)
                {
                    //if (@case.PreliminaryDecisionDocumentId != id) throw new ArgumentException();
                    @case.PreliminaryDecisionDocument.Name = name;
                }
                else
                {
                    var document = context.Documents.Create();
                    document.Id = id;
                    document.Name = name;
                    context.Documents.Add(document);
                    @case.PreliminaryDecisionDocumentId = id;
                }
                context.SaveChanges();
            }
        }

        public void AddFinalDecisionDocument(int caseId, Guid id, string name)
        {
            using (var context = new DatabaseContext())
            {
                var @case = context.Cases.FirstOrDefault(x => x.Id == caseId);
                if (@case == null) throw new Facade.CaseDoesNotExistsException();

                if (@case.FinalDecisionDocumentId != null)
                {
                    //if (@case.FinalDecisionDocumentId != id) throw new ArgumentException();
                    @case.FinalDecisionDocument.Name = name;
                }
                else
                {
                    var document = context.Documents.Create();
                    document.Id = id;
                    document.Name = name;
                    context.Documents.Add(document);
                    @case.FinalDecisionDocumentId = id;
                }
                context.SaveChanges();
            }
        }

        public Document[] GetAllDocuments(int caseId)
        {
            using (var context = new DatabaseContext())
            {
                var @case = context.Cases.FirstOrDefault(x => x.Id == caseId);
                if (@case == null) throw new Facade.CaseDoesNotExistsException();
                var documents = new[]
                {@case.FinalDecisionDocument, @case.PreliminaryDecisionDocument,}
                    .Concat(@case.Allegations.OrderBy(x => x.Authorship.Date).ThenBy(x => x.Order).SelectMany(x => x.Documents))
                    .Concat(@case.PartiesComments.OrderBy(x => x.Date).SelectMany(x => x.Documents))
                    .Concat(@case.DatesAndDetails.OrderBy(x => x.Date).ThenBy(x => x.Order).SelectMany(x => x.Documents))
                    .Where(x => x != null)
                    .Select(x => new Document
                    {
                        Id = x.Id,
                        Name = x.Name,
                    });
                var result = documents.ToArray();
                return result;
            }
        }

        public Document GetDocumentById(Guid documentId)
        {
            using (var context = new DatabaseContext())
            {
                var document = context.Documents.FirstOrDefault(x => x.Id == documentId);
                return new Document
                {
                    Id = document.Id,
                    Name = document.Name,
                };
            }
        }

        public int[] GetOpenCasesOlderThan(DateTime minimumStartDate)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Cases
                    .Where(x => x.Status == CaseEntity.CaseStatus.Open && x.StartDate < minimumStartDate)
                    .Select(x => x.Id);

                var result = query.ToArray();

                return result;
            }
        }

        public void SetClosedState(int[] caseIds)
        {
            using (var context = new DatabaseContext())
            {
                foreach (var caseId in caseIds)
                {
                    var entity = context.Cases.FirstOrDefault(x => x.Id == caseId);
                    entity.Status = CaseEntity.CaseStatus.Closed;
                }
                context.SaveChanges();
            }
        }

        public void SetCaseStartDate(int caseId, DateTime date)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.Cases.FirstOrDefault(x => x.Id == caseId);
                if (entity == null) throw new Facade.CaseDoesNotExistsException();
                entity.StartDate = date;
                context.SaveChanges();
            }
        }

        public Document GetFinalDecisionDocument(int caseId)
        {
            using (var context = new DatabaseContext())
            {
                var @case = context.Cases.FirstOrDefault(c => c.Id == caseId);
                if (@case == null) throw new Facade.CaseDoesNotExistsException();
                var doc = @case.FinalDecisionDocument;
                if (doc == null) return null;
                return new Document
                {
                    Id = doc.Id,
                    Name = doc.Name,
                };
            }
        }

        public Document[] GetDateAndDetailDocument(Guid dateAndDetailId)
        {
            using (var context = new DatabaseContext())
            {
                var dateAndDetail = context.DatesAndDetails.FirstOrDefault(x => x.Id == dateAndDetailId);
                if (dateAndDetail == null) return null;
                var result = dateAndDetail.Documents.Select(x => new Document {Id = x.Id, Name = x.Name}).ToArray();
                return result;
            }
        }

        public void DeleteDateAndDetail(Guid id)
        {
            using (var context = new DatabaseContext())
            {
                var dateAndDetail = context.DatesAndDetails.FirstOrDefault(x => x.Id == id);
                if (dateAndDetail == null) return;
                var documents = dateAndDetail.Documents.ToArray();
                foreach (var document in documents)
                {
                    dateAndDetail.Documents.Remove(document);
                    context.Documents.Remove(document);
                }
                context.DatesAndDetails.Remove(dateAndDetail);
                context.SaveChanges();
            }
        }

        public Document GetPreliminaryDecisionDocument(int caseId)
        {
            using (var context = new DatabaseContext())
            {
                var @case = context.Cases.FirstOrDefault(x => x.Id == caseId);
                if (@case == null) return null;
                var document = @case.PreliminaryDecisionDocument;
                if (document == null) return null;
                return new Document
                {
                    Id = document.Id,
                    Name = document.Name,
                };
            }
        }

        public bool GetIsLooseUser(Guid id)
        {
            using (var context = new DatabaseContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Id == id);
                if (user == null) return true;
                if (user.FromMessages != null && user.FromMessages.Count > 0) return false;
                if (user.Posts != null && user.Posts.Count > 0) return false;
                //if (user.AssignedCases != null && user.AssignedCases.Count > 0) return false;
                if (user.FinalDecisionComments != null && user.FinalDecisionComments.Count > 0) return false;
                if (user.PreliminaryDecisionComments != null && user.PreliminaryDecisionComments.Count > 0) return false;
                if (user.Schedule1 != null && user.Schedule1.Count > 0) return false;
                if (user.Schedule2 != null && user.Schedule2.Count > 0) return false;
                if (user.Schedule3 != null && user.Schedule3.Count > 0) return false;
                return true;
            }
        }

        public void AddDocumentActivity(DocumentActivity documentActivity)
        {
            using (var context = new DatabaseContext())
            {
                var entity = context.DocumentActivities.Create();
                entity.Id = documentActivity.Id;
                entity.CaseId = documentActivity.CaseId;
                entity.DocumentId = documentActivity.DocumentId;
                entity.DocumentName = documentActivity.DocumentName;
                entity.DocumentType = documentActivity.DocumentType;
                entity.ActionType = documentActivity.ActionType;
                entity.UserId = documentActivity.UserId;
                entity.Date = documentActivity.Date;
                context.DocumentActivities.Add(entity);
                context.SaveChanges();
            }
        }

        public DocumentActivity[] GetDocumentActivities(int caseId)
        {
            using (var context = new DatabaseContext())
            {
                var result = context.DocumentActivities.Where(x => x.CaseId == caseId).ToArray();
                return result.Cast<DocumentActivity>().ToArray();
            }
        }

        private string GetAllegationActivityFeedItemDescription(AllegationCommentType type, string text, string additionalText)
        {
            if (type == AllegationCommentType.Yes) return string.Format("Yes. {0}", text);
            if (type == AllegationCommentType.No) return string.Format("No. {0}", text);
            if (type == AllegationCommentType.Advise) return string.Format("Advice. {0}\n{1}", text, additionalText);
            throw new ArgumentException();
        }

        private static void SaveNewSchedules(Schedule[] newSchedules, DatabaseContext context)
        {
            foreach (var schedule in newSchedules)
            {
                var scheduleEntity = new ScheduleEntity {Id = schedule.Id};
                context.Schedules.Add(scheduleEntity);
            }
            context.SaveChanges();
        }

        private static Schedule[] CreateSchedules()
        {
            var newSchedules = new Schedule[10];
            for (int i = 0; i < newSchedules.Length; ++i)
            {
                var schedule = new Schedule {Id = i};
                newSchedules[i] = schedule;
            }
            return newSchedules;
        }

        private static Schedule[] SelectSchedules(DatabaseContext context)
        {
            var entities =
                context.Schedules.Include(item => item.User1).Include(item => item.User2).Include(item => item.User3).ToArray();
            var result = entities.Select(item => new Schedule
            {
                Id = item.Id,
                First = item.User1,
                Second = item.User2,
                Third = item.User3,
            }).ToArray();
            return result;
        }
    }
}