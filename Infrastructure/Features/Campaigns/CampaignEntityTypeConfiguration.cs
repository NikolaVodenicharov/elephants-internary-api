using Core.Features.Campaigns.Entities;
using Core.Features.Campaigns.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Features.Campaigns
{
    public class CampaignEntityTypeConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.HasKey(c => c.Id);

            builder
                .Property(c => c.Name)
                .IsRequired(true)
                .HasMaxLength(CampaignValidationConstants.NameMaxLength);

            builder
                .Property(c => c.StartDate)
                .IsRequired(true);

            builder
                .Property(c => c.EndDate)
                .IsRequired(true);

            builder
                .Property(c => c.IsActive)
                .IsRequired(true);
        }   
    }
}
