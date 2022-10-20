using Core.Features.Persons.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Persons
{
    internal class PersonRoleEntityTypeConfiguration : IEntityTypeConfiguration<PersonRole>
    {
        public void Configure(EntityTypeBuilder<PersonRole> builder)
        {
            builder
                .HasKey(pr => new { pr.RoleId, pr.PersonId });

            builder
                .HasOne(pr => pr.Role)
                .WithMany(r => r.PersonRoles)
                .HasForeignKey(pr => pr.RoleId);

            builder
                .HasOne(pr => pr.Person)
                .WithMany(p => p.PersonRoles)
                .HasForeignKey(pr => pr.PersonId);
        }
    }
}
