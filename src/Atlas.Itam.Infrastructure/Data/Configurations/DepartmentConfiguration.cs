using Atlas.Itam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Itam.Infrastructure.Data.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.DepartmentId);

        builder.Property(d => d.DepartmentId)
            .HasColumnName("department_id")
            .HasColumnType("uuid");

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(100)")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasIndex(d => d.Name)
            .IsUnique()
            .HasDatabaseName("uk_departments_name");
    }
}
