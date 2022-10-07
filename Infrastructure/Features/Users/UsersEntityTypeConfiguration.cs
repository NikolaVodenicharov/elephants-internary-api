using Core.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Users
{
    internal class UsersEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder
                .Property(u => u.Email)
                .IsRequired(true);
            
            builder
                .Property(u => u.RoleId);
            
            builder
                .HasOne(u => u.Mentor)
                .WithOne()
                .IsRequired(false);
        }
    }
}