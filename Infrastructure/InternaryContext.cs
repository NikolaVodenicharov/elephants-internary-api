using Core.Features.Campaigns.Entities;
using Core.Features.Mentors.Entities;
using Core.Features.Specialties.Entities;
using Infrastructure.Features.Campaigns;
using Infrastructure.Features.Mentors;
using Infrastructure.Features.Specialities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class InternaryContext : DbContext
    {
        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Mentor> Mentors { get; set; }

        public DbSet<Speciality> Specialties { get; set; }

        public InternaryContext(DbContextOptions<InternaryContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CampaignEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SpecialityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MentorEntityTypeConfiguration());
        }
    }
}