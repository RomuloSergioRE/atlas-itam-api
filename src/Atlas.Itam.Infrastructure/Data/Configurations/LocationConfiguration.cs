using Atlas.Itam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Itam.Infrastructure.Data.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.LocationId);

        builder.Property(l => l.LocationId)
            .HasColumnName("location_id")
            .HasColumnType("uuid");

        builder.Property(l => l.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(150)")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(l => l.Address)
            .HasColumnName("address")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255);

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .IsRequired();
    }
}
