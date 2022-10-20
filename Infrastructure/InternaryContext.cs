using Core.Features.Campaigns.Entities;
using Core.Features.Specialties.Entities;
using Core.Features.LearningTopics.Entities;
using Core.Features.Interns.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Core.Features.Persons.Entities;
using Infrastructure.Common;

namespace Infrastructure
{
    public class InternaryContext : DbContext
    {
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Speciality> Specialties { get; set; }
        public DbSet<InternCampaign> InternCampaigns { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<LearningTopic> LearningTopics { get; set; }       
        public DbSet<Role> Roles { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonRole> PersonRoles { get; set; }

        public InternaryContext(DbContextOptions<InternaryContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.ConfigureShadowProperties();
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