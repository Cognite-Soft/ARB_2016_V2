using System.Collections.Generic;
using Cognite.Arb.Server.Business.Database;

namespace Cognite.Arb.Server.Resource.Database
{
    public class UserEntity : UserData
    {
        public UserEntity()
        {
            ResetTokens = new List<ResetTokenEntity>();
            AssignedCases = new List<CaseEntity>();
            Schedule1 = new List<ScheduleEntity>();
            Schedule2 = new List<ScheduleEntity>();
            Schedule3 = new List<ScheduleEntity>();
            PreliminaryDecisionComments = new List<PreliminaryDecisionCommentEntity>();
            FinalDecisionComments = new List<FinalDecisionCommentEntity>();
            FromMessages = new List<MessageEntity>();
        }

        public UserEntity(UserData user)
        {
            Id = user.Id;
            Email = user.Email;
            Update(user);
        }

        public void Update(UserData user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Role = user.Role;
            HashedPassword = user.HashedPassword;
            PasswordSalt = user.PasswordSalt;
            EncryptedSecurePhrase = user.EncryptedSecurePhrase;
            FirstSecurePhraseQuestionCharacterIndex = user.FirstSecurePhraseQuestionCharacterIndex;
            SecondSecurePhraseQuestionCharacterIndex = user.SecondSecurePhraseQuestionCharacterIndex;
            UserState = user.UserState;
        }

        public virtual ICollection<ResetTokenEntity> ResetTokens { get; set; }
        public virtual ICollection<CaseEntity> AssignedCases { get; set; }
        public ICollection<ScheduleEntity> Schedule1 { get; set; }
        public ICollection<ScheduleEntity> Schedule2 { get; set; }
        public ICollection<ScheduleEntity> Schedule3 { get; set; }
        public virtual ICollection<PreliminaryDecisionCommentEntity> PreliminaryDecisionComments { get; set; }
        public virtual ICollection<FinalDecisionCommentEntity> FinalDecisionComments { get; set; }
        public virtual ICollection<MessageEntity> FromMessages { get; set; }
        public virtual ICollection<PostEntity> Posts { get; set; } 
    }
}
