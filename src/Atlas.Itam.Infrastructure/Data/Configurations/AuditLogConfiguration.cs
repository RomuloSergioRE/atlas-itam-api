using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Itam.Infrastructure.Data.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(a => a.LogId);

        builder.Property(a => a.LogId)
            .HasColumnName("log_id")
            .HasColumnType("uuid");

        builder.Property(a => a.UserId)
            .HasColumnName("user_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(a => a.Action)
            .HasColumnName("action")
            .HasColumnType("varchar(20)")
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<AuditAction>(v, true));

        builder.Property(a => a.EntityName)
            .HasColumnName("entity_name")
            .HasColumnType("varchar(50)")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.EntityId)
            .HasColumnName("entity_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(a => a.OldValues)
            .HasColumnName("old_values")
            .HasColumnType("jsonb");

        builder.Property(a => a.NewValues)
            .HasColumnName("new_values")
            .HasColumnType("jsonb");

        builder.Property(a => a.IpAddress)
            .HasColumnName("ip_address")
            .HasColumnType("varchar(45)")
            .HasMaxLength(45);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.UserId)
            .HasDatabaseName("idx_audit_user");

        builder.HasIndex(a => new { a.EntityName, a.EntityId })
            .HasDatabaseName("idx_audit_entity");

        builder.HasIndex(a => a.Action)
            .HasDatabaseName("idx_audit_action");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("idx_audit_created");
    }
}
