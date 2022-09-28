using Core.Features.Interns.Entities;
using Core.Features.Interns.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Interns
{
    public class StateEntityTypeConfiguration : IEntityTypeConfiguration<State>
    {
        public void Configure(EntityTypeBuilder<State> builder)
        {
            builder
                .HasKey(s => s.Id);

            builder
                .Property(s => s.Justification)
                .HasMaxLength(InternValidationConstants.JustificationMaxLength);

            builder
                .Property(s => s.Created);

            builder
                .Property(s => s.StatusId)
                .HasConversion<int>();

            builder
                .HasOne(s => s.InternCampaign)
                .WithMany(i => i.States)
                .HasForeignKey(s => new { s.InternId, s.CampaignId });
        }
    }
}
