using Core.Features.Campaigns.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Campaigns
{
    public class CampaignTypeEntityTypeConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            throw new NotImplementedException();
        }
    }
}
