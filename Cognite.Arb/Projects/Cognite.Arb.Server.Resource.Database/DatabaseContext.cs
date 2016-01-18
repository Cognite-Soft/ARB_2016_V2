using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Cognite.Arb.Server.Resource.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<ResetTokenEntity> ResetTokens { get; set; }
        public DbSet<CaseEntity> Cases { get; set; }
        public DbSet<ScheduleEntity> Schedules { get; set; }
        public DbSet<DateAndDetailEntity> DatesAndDetails { get; set; }
        public DbSet<AllegationEntity> Allegations { get; set; }
        public DbSet<PreliminaryDecisionCommentEntity> PreliminaryDecisionComments { get; set; }
        public DbSet<FinalDecisionCommentEntity> FinalDecisionComments { get; set; }
        public DbSet<PostEntity> Posts { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<DocumentEntity> Documents { get; set; }
        public DbSet<PartiesCommentEntity> PartiesComments { get; set; }
        public DbSet<DocumentActivityEntity> DocumentActivities { get; set; }

        public static string OverrideConnectionString { get; set; }

        static DatabaseContext()
        {
            // Hack for assembly linker to see that we use EntityFramework.SqlServer.dll and copy it to out folder in other projects
            // ReSharper disable once UnusedVariable
            var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public DatabaseContext()
            : base(OverrideConnectionString ?? "name=DatabaseConnection")
        {
//#if DEBUG
//            Database.Log = s => Debug.WriteLine(s);
//#endif
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<StoreGeneratedIdentityKeyConvention>();
            modelBuilder.Configurations.Add(new UserEntityConfiguration());
            modelBuilder.Configurations.Add(new ResetPasswordTokenEntityConfiguration());
            modelBuilder.Configurations.Add(new CaseEntityConfiguration());
            modelBuilder.Configurations.Add(new ScheduleEntityConfiguration());
            modelBuilder.Configurations.Add(new DateAndDetailEntityConfiguration());
            modelBuilder.Configurations.Add(new AllegationEntityConfiguration());
            modelBuilder.Configurations.Add(new AllegationCommentEntityConfiguration());
            modelBuilder.Configurations.Add(new PreliminaryDecisionCommentEntityConfiguration());
            modelBuilder.Configurations.Add(new FinalDecisionCommentEntityConfiguration());
            modelBuilder.Configurations.Add(new PostEntityConfiguration());
            modelBuilder.Configurations.Add(new MessageEntityConfiguration());
            modelBuilder.Configurations.Add(new DocumentEntityConfiguration());
            modelBuilder.Configurations.Add(new PartiesCommentEntityConfiguration());
            modelBuilder.Configurations.Add(new DocumentActivityEntityConfiguration());
        }

        private class UserEntityConfiguration : EntityTypeConfiguration<UserEntity>
        {
            public UserEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".User");
                HasKey(x => x.Id);

                Property(x => x.Email).IsRequired().HasMaxLength(200)
                    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Email") {IsUnique = true}));
                Property(x => x.Role).IsRequired();
                Property(x => x.UserState).IsRequired();

                Property(x => x.FirstName).IsOptional().HasMaxLength(100);
                Property(x => x.LastName).IsOptional().HasMaxLength(100);
                Property(x => x.HashedPassword).HasColumnName("Password").IsOptional().HasMaxLength(44);
                Property(x => x.PasswordSalt).IsOptional().HasMaxLength(44);
                Property(x => x.EncryptedSecurePhrase).HasColumnName("SecurePhrase").IsOptional().HasMaxLength(256);
                Property(x => x.FirstSecurePhraseQuestionCharacterIndex).HasColumnName("SecurePhraseQuestion1").IsOptional();
                Property(x => x.SecondSecurePhraseQuestionCharacterIndex).HasColumnName("SecurePhraseQuestion2").IsOptional();
            }
        }

        private class ResetPasswordTokenEntityConfiguration : EntityTypeConfiguration<ResetTokenEntity>
        {
            public ResetPasswordTokenEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".ResetToken");
                HasKey(x => x.Token);

                Property(x => x.Token).IsRequired().HasMaxLength(38);
                Property(x => x.UserId).IsRequired();
                Property(x => x.ExpirationTime).IsRequired();
                Property(x => x.Type).IsRequired();

                HasRequired(x => x.User).WithMany(x => x.ResetTokens);
            }
        }

        private class CaseEntityConfiguration : EntityTypeConfiguration<CaseEntity>
        {
            public CaseEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".Case");
                HasKey(x => x.Id);

                Property(x => x.Id).IsRequired();
                Property(x => x.Status).IsRequired();
                Property(x => x.StartDate).IsRequired().HasColumnType("datetime2");

                Property(x => x.ParentId).IsOptional();
                Property(x => x.Background).IsOptional();
                Property(x => x.IdealOutcome).IsOptional();
                Property(x => x.Relationship).IsOptional();
                Property(x => x.ContactAgreement).IsOptional();

                Property(x => x.IssueRaisedWithArchitect.Answer).IsOptional();
                Property(x => x.IssueRaisedWithArchitect.Comments).IsOptional();
                Property(x => x.SubjectOfLegalProceedings.Answer).IsOptional();
                Property(x => x.SubjectOfLegalProceedings.Comments).IsOptional();
                Property(x => x.ClaimantContact.Name).IsOptional();
                Property(x => x.ClaimantContact.RegistrationNumber).IsOptional();
                Property(x => x.ClaimantContact.EMail).IsOptional();
                Property(x => x.ClaimantContact.Address).IsOptional();
                Property(x => x.ClaimantContact.Phone).IsOptional();
                Property(x => x.ArchitectContact.Name).IsOptional();
                Property(x => x.ArchitectContact.RegistrationNumber).IsOptional();
                Property(x => x.ArchitectContact.EMail).IsOptional();
                Property(x => x.ArchitectContact.Address).IsOptional();
                Property(x => x.ArchitectContact.Phone).IsOptional();

                Property(x => x.ProcessStartDate).IsOptional().HasColumnType("datetime2");

                HasMany(x => x.AssignedUsers).WithMany(x => x.AssignedCases).Map(x =>
                {
                    x.MapRightKey("UserId");
                    x.MapLeftKey("CaseId");
                    x.ToTable("CaseUser");
                });

                Property(x => x.PreliminaryDecisionDocumentId).IsOptional();
                Property(x => x.FinalDecisionDocumentId).IsOptional();
                //Property(x => x.FinalDecisionSubmitDate).IsOptional();
                HasOptional(x => x.PreliminaryDecisionDocument)
                    .WithMany(x => x.PreliminaryDecisionCases)
                    .HasForeignKey(x => x.PreliminaryDecisionDocumentId);
                HasOptional(x => x.FinalDecisionDocument)
                    .WithMany(x => x.FinalDecisionCases)
                    .HasForeignKey(x => x.FinalDecisionDocumentId);
            }
        }

        private class ScheduleEntityConfiguration : EntityTypeConfiguration<ScheduleEntity>
        {
            public ScheduleEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".Schedule");
                HasKey(x => x.Id);

                Property(x => x.User1Id);
                Property(x => x.User2Id);
                Property(x => x.User3Id);
                Property(x => x.LastUsed);

                HasOptional(x => x.User1).WithMany(x => x.Schedule1).HasForeignKey(x => x.User1Id);
                HasOptional(x => x.User2).WithMany(x => x.Schedule2).HasForeignKey(x => x.User2Id);
                HasOptional(x => x.User3).WithMany(x => x.Schedule3).HasForeignKey(x => x.User3Id);
            }
        }

        private class DateAndDetailEntityConfiguration : EntityTypeConfiguration<DateAndDetailEntity>
        {
            public DateAndDetailEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".DateAndDetail");
                HasKey(x => x.Id);
                Property(x => x.Text).IsRequired();
                Property(x => x.Date).IsRequired().HasColumnType("datetime2");
                Property(x => x.Order).IsRequired();
                
                HasRequired(x => x.Case).WithMany(x => x.DatesAndDetails).HasForeignKey(x => x.CaseId);
                Property(x => x.Authorship.Date).IsRequired().HasColumnType("datetime2");
                Property(x => x.Authorship.UserId).IsRequired();
            }
        }

        private class AllegationEntityConfiguration : EntityTypeConfiguration<AllegationEntity>
        {
            public AllegationEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".Allegation");
                HasKey(x => x.Id);
                Property(x => x.Text).IsRequired();
                HasRequired(x => x.Case).WithMany(x => x.Allegations).HasForeignKey(x => x.CaseId);
                Property(x => x.Authorship.Date).IsRequired().HasColumnType("datetime2");
                Property(x => x.Authorship.UserId).IsRequired();
                Property(x => x.Order).IsRequired();
            }
        }

        private class AllegationCommentEntityConfiguration : EntityTypeConfiguration<AllegationCommentEntity>
        {
            public AllegationCommentEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".AllegationComment");
                HasKey(x => x.Id);
                Property(x => x.Text).IsRequired();
                Property(x => x.AdditionalText).IsOptional();
                Property(x => x.Authorship.Date).IsRequired().HasColumnType("datetime2");
                Property(x => x.Authorship.UserId).IsRequired();
                HasRequired(x => x.Allegation).WithMany(x => x.Comments).HasForeignKey(x => x.AllegationId);
            }
        }

        private class PreliminaryDecisionCommentEntityConfiguration : EntityTypeConfiguration<PreliminaryDecisionCommentEntity>
        {
            public PreliminaryDecisionCommentEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".PreliminatyDecisionComment");
                HasKey(x => x.Id);
                Property(x => x.CaseId).IsRequired();
                Property(x => x.PanelMemberId).IsRequired();
                Property(x => x.Date).HasColumnType("datetime2");
                Property(x => x.Text).IsRequired();
                HasRequired(x => x.Case).WithMany(x => x.PreliminaryDecisionComments).HasForeignKey(x => x.CaseId);
                HasRequired(x => x.PanelMember).WithMany(x => x.PreliminaryDecisionComments).HasForeignKey(x => x.PanelMemberId);
            }
        }

        private class FinalDecisionCommentEntityConfiguration : EntityTypeConfiguration<FinalDecisionCommentEntity>
        {
            public FinalDecisionCommentEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".FinalDecisionComment");
                HasKey(x => x.Id);
                Property(x => x.CaseId).IsRequired();
                Property(x => x.PanelMemberId).IsRequired();
                Property(x => x.Date).HasColumnType("datetime2");
                Property(x => x.Decision).IsRequired();
                Property(x => x.CommentForChange).IsOptional();
                HasRequired(x => x.Case).WithMany(x => x.FinalDecisionComments).HasForeignKey(x => x.CaseId);
                HasRequired(x => x.PanelMember).WithMany(x => x.FinalDecisionComments).HasForeignKey(x => x.PanelMemberId);
            }
        }

        private class PostEntityConfiguration : EntityTypeConfiguration<PostEntity>
        {
            public PostEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".Post");
                HasKey(x => x.Id);
                Property(x => x.CaseId);
                Property(x => x.UserId);
                Property(x => x.Text);
                Property(x => x.Date).HasColumnType("datetime2");
                HasOptional(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId);
                HasRequired(x => x.User).WithMany(x => x.Posts).HasForeignKey(x => x.UserId);
            }
        }

        private class MessageEntityConfiguration : EntityTypeConfiguration<MessageEntity>
        {
            public MessageEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".Message");
                HasKey(x => x.Id);
                Property(x => x.FromUserId).IsRequired();
                Property(x => x.ToUserId).IsRequired();
                Property(x => x.Text).IsRequired();
                Property(x => x.Created).IsRequired().HasColumnType("datetime2");
                Property(x => x.Accepted).IsOptional().HasColumnType("datetime2");
                HasRequired(x => x.FromUser).WithMany(x => x.FromMessages).HasForeignKey(x => x.FromUserId);
            }
        }

        private class DocumentEntityConfiguration : EntityTypeConfiguration<DocumentEntity>
        {
            public DocumentEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".Doument");
                HasKey(x => x.Id);
                Property(x => x.Name).IsRequired();

                HasMany(x => x.Allegations).WithMany(x => x.Documents).Map(x =>
                {
                    x.MapRightKey("AllegationId");
                    x.MapLeftKey("DocumentId");
                    x.ToTable("AllegationDocument");
                });
                HasMany(x => x.DateAndDetails).WithMany(x => x.Documents).Map(x =>
                {
                    x.MapRightKey("DateAndDetailId");
                    x.MapLeftKey("DocumentId");
                    x.ToTable("DateAndDetailDocument");
                });
            }
        }

        private class PartiesCommentEntityConfiguration : EntityTypeConfiguration<PartiesCommentEntity>
        {
            public PartiesCommentEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".PartiesComment");
                HasKey(x => x.Id);
                Property(x => x.CaseId).IsRequired();
                Property(x => x.Comment).IsOptional();
                Property(x => x.Date).IsRequired();
                HasRequired(x => x.Case).WithMany(x => x.PartiesComments).HasForeignKey(x => x.CaseId);
                HasMany(x => x.Documents).WithMany(x => x.PartiesComments).Map(x =>
                {
                    x.MapRightKey("DocumentId");
                    x.MapLeftKey("PartyCommentId");
                    x.ToTable("PartyCommentDocument");
                });
            }
        }

        private class DocumentActivityEntityConfiguration : EntityTypeConfiguration<DocumentActivityEntity>
        {
            public DocumentActivityEntityConfiguration(string schema = "dbo")
            {
                ToTable(schema + ".DocumentActivity");
                HasKey(x => x.Id);
                Property(x => x.CaseId).IsRequired();
                Property(x => x.DocumentId).IsRequired();
                Property(x => x.DocumentName).IsRequired();
                Property(x => x.DocumentType).IsRequired();
                Property(x => x.ActionType).IsRequired();
                Property(x => x.UserId).IsRequired();
                Property(x => x.Date).IsRequired();
            }
        }
    }
}