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
        public DbSet<Campaign> Campaigns { get; set; } = null!;
        public DbSet<Speciality> Specialties { get; set; } = null!;
        public DbSet<InternCampaign> InternCampaigns { get; set; } = null!;
        public DbSet<State> States { get; set; } = null!;
        public DbSet<Status> Status { get; set; } = null!;
        public DbSet<LearningTopic> LearningTopics { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<PersonRole> PersonRoles { get; set; } = null!;

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