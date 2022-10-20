using Core.Features.Persons.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Persons
{
    internal class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder
                .Property(r => r.RoleId)
                .HasConversion<int>();

            builder
                .HasData(
                    Enum.GetValues(typeof(RoleId))
                    .Cast<RoleId>()
                    .Select(roleId => new Role()
                    {
                        RoleId = roleId,
                        Name = roleId.ToString()
                    })
                );
        }
    }
}
