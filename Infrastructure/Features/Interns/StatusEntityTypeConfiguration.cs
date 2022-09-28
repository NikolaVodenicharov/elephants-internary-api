using Core.Features.Interns.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Features.Interns
{
    public class StatusEntityTypeConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder
                .Property(s => s.StatusId)
                .HasConversion<int>();

            builder
                .HasData(
                    Enum.GetValues(typeof(StatusEnum))
                    .Cast<StatusEnum>()
                    .Select(statusId => new Status()
                    {
                        StatusId = statusId,
                        Name = statusId.ToString()
                    })
                );
        }
    }
}
