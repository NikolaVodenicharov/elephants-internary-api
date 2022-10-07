using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Common
{
    internal static class ShadowPropertiesConfiguration
    {
        public static void ConfigureShadowProperties(this ModelBuilder modelBuilder)
        {
            var allEntities = modelBuilder.Model.GetEntityTypes();

            foreach (var entity in allEntities)
            {
                entity.AddProperty("CreatedDate", typeof(DateTime));
                entity.AddProperty("UpdatedDate", typeof(DateTime));
            }
        }

        public static void SetShadowPropertiesDateValues(this ChangeTracker changeTracker)
        {
            var addedEntities = changeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();
            var editedEntities = changeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();

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
