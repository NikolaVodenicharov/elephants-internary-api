using Core.Features.Campaigns.Entities;
using Core.Features.Mentors.Entities;
using Core.Features.Mentors.Support;
using Core.Features.Specialties.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Mentors
{
    internal class MentorEntityTypeConfiguration : IEntityTypeConfiguration<Mentor>
    {
        public void Configure(EntityTypeBuilder<Mentor> builder)
        {
            builder.HasKey(m => m.Id);

            builder
                .Property(m => m.DisplayName)
                .IsRequired(true);

            builder
                .Property(m => m.Email)
                .IsRequired(true);

            builder
                .HasMany(m => m.Specialities)
                .WithMany(s => s.Mentors)
                .UsingEntity<Dictionary<string, object>>(
                    "MentorSpecialties",
                    b => b.HasOne<Speciality>().WithMany()
                            .HasForeignKey("SpecialityId").HasConstraintName("FK_SpecialityId")
                            .OnDelete(DeleteBehavior.NoAction),
                    b => b.HasOne<Mentor>().WithMany()
                            .HasForeignKey("MentorId").HasConstraintName("FK_MentorId")
                            .OnDelete(DeleteBehavior.NoAction)
                );

            builder
                .HasMany(m => m.Campaigns)
                .WithMany(c => c.Mentors)
                .UsingEntity<Dictionary<string, object>>(
                    "CampaignMentors",
                    b => b.HasOne<Campaign>().WithMany().HasForeignKey("CampaignId").HasConstraintName("CampaignId"),
                    b => b.HasOne<Mentor>().WithMany().HasForeignKey("MentorId").HasConstraintName("MentorId")
                );
        }
    }
}
