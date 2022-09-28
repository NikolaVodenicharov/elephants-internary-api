using Core.Features.Interns.Entities;
using Core.Features.Interns.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Interns
{
    public class InternEntityTypeConfiguration : IEntityTypeConfiguration<Intern>
    {
        public void Configure(EntityTypeBuilder<Intern> builder)
        {
            builder
                .HasKey(i => i.Id);

            builder
                .Property(i => i.FirstName)
                .IsRequired()
                .HasMaxLength(InternValidationConstants.InternNameMaxLength);

            builder
                .Property(i => i.LastName)
                .IsRequired()
                .HasMaxLength(InternValidationConstants.InternNameMaxLength);

            builder
                .Property(i => i.PersonalEmail)
                .IsRequired();

            builder
                .HasMany(i => i.InternCampaigns)
                .WithOne(ic => ic.Intern)
                .HasForeignKey(ic => ic.InternId);
        }
    }
}
