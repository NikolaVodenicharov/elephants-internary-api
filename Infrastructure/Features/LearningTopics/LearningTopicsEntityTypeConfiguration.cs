using Core.Features.LearningTopics.Entities;
using Core.Features.Specialties.Entities;
using Core.Features.LearningTopics.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.LearningTopics
{
    public class LearningTopicEntityTypeConfiguration : IEntityTypeConfiguration<LearningTopic>
    {
        public void Configure(EntityTypeBuilder<LearningTopic> builder)
        {
            builder.HasKey(t => t.Id);

            builder
                .Property(t => t.Name)
                .IsRequired(true)
                .HasMaxLength(LearniningTopicValidationConstants.NameMaxLength);
            
            builder
                .HasMany(t => t.Specialities)
                .WithMany(s => s.LearningTopics)
                .UsingEntity<Dictionary<string, object>>(
                    "LearningTopicSpecialities",
                    b => b.HasOne<Speciality>().WithMany()
                        .HasForeignKey("SpecialityId")
                        .HasConstraintName("FK_LearningTopic_SpecialityId")
                        .OnDelete(DeleteBehavior.NoAction),
                    b => b.HasOne<LearningTopic>().WithMany()
                        .HasForeignKey("LearningTopicId")
                        .HasConstraintName("FK_LearningTopicId")
                        .OnDelete(DeleteBehavior.NoAction)
                );
        }
    }
}