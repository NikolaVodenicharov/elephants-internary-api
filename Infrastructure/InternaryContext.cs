using Core.Features.Campaigns.Entities;
using Core.Features.Mentors.Entities;
using Core.Features.Specialties.Entities;
using Core.Features.LearningTopics.Entities;
using Infrastructure.Features.Campaigns;
using Infrastructure.Features.Mentors;
using Infrastructure.Features.Specialities;
using Infrastructure.Features.LearningTopics;
using Core.Features.Interns.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure
{
    public class InternaryContext : DbContext
    {
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Speciality> Specialties { get; set; }
        public DbSet<Intern> Interns { get; set; }
        public DbSet<InternCampaign> InternCampaigns { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<LearningTopic> LearningTopics { get; set; }

        public InternaryContext(DbContextOptions<InternaryContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var allEntities = modelBuilder.Model.GetEntityTypes();

            foreach (var entity in allEntities)
            {
                if (entity.GetTableName() == "Status")
                    continue;

                entity.AddProperty("CreatedDate", typeof(DateTime));
                entity.AddProperty("UpdatedDate", typeof(DateTime));
            }

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override int SaveChanges()
        {
            Save();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Save();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void Save()
        {
            var addedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();
            var editedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();

            var utcNow = DateTime.UtcNow;

            addedEntities.ForEach(e =>
            {
                if (e.Properties.Any(p => p.Metadata.Name == "CreatedDate"))
                {
                    e.Property("CreatedDate").CurrentValue = utcNow;
                    e.Property("UpdatedDate").CurrentValue = utcNow;
                }
            });

            editedEntities.ForEach(e =>
            {
                if (e.Properties.Any(p => p.Metadata.Name == "CreatedDate"))
                {
                    e.Property("CreatedDate").IsModified = false;
                }

                if (e.Properties.Any(p => p.Metadata.Name == "UpdatedDate"))
                {
                    e.Property("UpdatedDate").CurrentValue = utcNow;
                }
            });
        }
    }
}