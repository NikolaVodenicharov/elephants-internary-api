using Core.Features.Campaigns.Entities;
using Infrastructure.Features.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class InternaryContext : DbContext
    {
        //public DbSet<Campaign> Campaigns { get; set; }

        public InternaryContext(DbContextOptions<InternaryContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new CampaignTypeEntityTypeConfiguration());
        }
    }
}