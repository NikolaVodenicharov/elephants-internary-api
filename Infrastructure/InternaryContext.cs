using Core.Features.Campaigns.Entities;
using Core.Features.Mentors.Entities;
using Core.Features.Specialties.Entities;
using Core.Features.LearningTopics.Entities;
using Core.Features.Interns.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Infrastructure.Common;
using Core.Features.Users.Entities;

namespace Infrastructure
{
    public class InternaryContext : DbContext
    {
        // Entities that have data seeding will not have shadow properties added
        private readonly List<string> entitiesToIgnore = new List<string>()
        {
            nameof(Status), nameof(Role)
        };

        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Speciality> Specialties { get; set; }
        public DbSet<Intern> Interns { get; set; }
        public DbSet<InternCampaign> InternCampaigns { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<LearningTopic> LearningTopics { get; set; }
        
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public InternaryContext(DbContextOptions<InternaryContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.ConfigureShadowProperties(entitiesToIgnore);
        }

        public override int SaveChanges()
        {
            ChangeTracker.SetShadowPropertiesDateValues();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.SetShadowPropertiesDateValues();

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}