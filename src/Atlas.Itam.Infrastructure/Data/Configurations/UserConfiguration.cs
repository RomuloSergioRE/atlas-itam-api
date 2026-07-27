using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Itam.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uuid");

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(150)")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasColumnType("varchar(255)")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasColumnType("varchar(255)")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasColumnType("varchar(20)")
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<UserRole>(v, true));

        builder.Property(u => u.DepartmentId)
            .HasColumnName("department_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("boolean")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasOne(u => u.Department)
            .WithMany()
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("idx_users_email");

        builder.HasIndex(u => u.DepartmentId)
            .HasDatabaseName("idx_users_department");
    }
}
