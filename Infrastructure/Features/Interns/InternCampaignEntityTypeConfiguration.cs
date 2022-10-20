using Core.Features.Interns.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Interns
{
    public class InternCampaignEntityTypeConfiguration : IEntityTypeConfiguration<InternCampaign>
    {
        public void Configure(EntityTypeBuilder<InternCampaign> builder)
        {
            builder
                .HasKey(i => new { i.PersonId, i.CampaignId});

            builder
                .HasOne(i => i.Person)
                .WithMany(intern => intern.InternCampaigns)
                .HasForeignKey(i => i.PersonId);

            builder
                .HasOne(i => i.Campaign)
                .WithMany(c => c.InternCampaigns)
                .HasForeignKey(i => i.CampaignId);

            builder
                .HasOne(i => i.Speciality)
                .WithMany(s => s.InternCampaigns)
                .HasForeignKey(i =>i.SpecialityId);
        }
    }
}
