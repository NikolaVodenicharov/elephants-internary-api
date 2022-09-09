using Core.Features.Specialities.Support;
using Core.Features.Specialties.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Specialities;

public class SpecialityEntityTypeConfiguration : IEntityTypeConfiguration<Speciality>
{
    public void Configure(EntityTypeBuilder<Speciality> builder)
    {
        builder.HasKey(s => s.Id);

        builder
            .Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(SpecialityValidationConstants.NameMaxLength);
    }
}
