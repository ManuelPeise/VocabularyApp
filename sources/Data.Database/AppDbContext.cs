using Data.Database.Entities;
using Data.Database.Entities.User;
using Data.Database.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;

namespace Data.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VocabularyToCategoryEntity>(entity =>
            {
                entity.HasIndex(e => new { e.VocabularyId, e.CategoryId }).IsUnique();

                entity.HasOne(e => e.Vocabulary)
                    .WithMany(v => v.VocabulatyToCategoryEntities)
                    .HasForeignKey(e => e.VocabularyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.VocabulariesToCategoryEntities)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VocabularySessionEntity>(entity =>
            {
                entity.HasMany(e => e.Vocabularies)
                    .WithMany()
                    .UsingEntity<Dictionary<string, object>>(
                        "VocabularySessionVocabulary",
                        j => j.HasOne<VocabularyEntity>()
                            .WithMany()
                            .HasForeignKey("VocabularyId")
                            .OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<VocabularySessionEntity>()
                            .WithMany()
                            .HasForeignKey("SessionId")
                            .OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.HasKey("SessionId", "VocabularyId");
                        });
            });

        }

        // user tabels
        public DbSet<UserEntity> UserTable { get; set; }
        public DbSet<UserCredentialsEntity> UserCredentialsTable { get; set; }
        public DbSet<UserSettingsEntity> UserSettingsTable { get; set; }
        public DbSet<LogMessageEntity> LogMessageTable { get; set; }

        // vocabulary tabels
        public DbSet<LanguageEntity> LanguageTable { get; set; }
        public DbSet<PartOfSpeechEntity> PartOfSpeachTable { get; set; }
        public DbSet<VocabularyEntity> VocabularyTable { get; set; }
        public DbSet<VocabularyCategoryEntity> VocabularyCategoryTable { get; set; }
        public DbSet<VocabularyToCategoryEntity> VocabularyToCategoriesTable { get; set; }
        public DbSet<VocabularySessionEntity> VocabularySessionTable { get; set; }
        public DbSet<VocabularySessionResultEntity> VocabularySessionResultTable { get; set; }
        public DbSet<VocabularyProgressEntity> VocabularyProgressTable { get; set; }

    }
}
