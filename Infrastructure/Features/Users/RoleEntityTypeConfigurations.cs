using Core.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Users
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
                    Enum.GetValues(typeof(RoleEnum))
                    .Cast<RoleEnum>()
                    .Select(roleId => new Role()
                    {
                        RoleId = roleId,
                        Name = roleId.ToString()
                    })
                );
        }
    }
}
