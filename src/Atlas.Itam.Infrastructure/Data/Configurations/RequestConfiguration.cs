using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Itam.Infrastructure.Data.Configurations;

public sealed class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("requests");

        builder.HasKey(r => r.RequestId);

        builder.Property(r => r.RequestId)
            .HasColumnName("request_id")
            .HasColumnType("uuid");

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue(RequestStatus.Pending)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<RequestStatus>(v, true));

        builder.Property(r => r.Justification)
            .HasColumnName("justification")
            .HasColumnType("text")
            .IsRequired();

        builder.Property(r => r.AssetId)
            .HasColumnName("asset_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(r => r.RequestedById)
            .HasColumnName("requested_by_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(r => r.ApprovedById)
            .HasColumnName("approved_by_id")
            .HasColumnType("uuid");

        builder.Property(r => r.ApprovedAt)
            .HasColumnName("approved_at")
            .HasColumnType("timestamp");

        builder.Property(r => r.RejectionReason)
            .HasColumnName("rejection_reason")
            .HasColumnType("text");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasOne(r => r.Asset)
            .WithMany()
            .HasForeignKey(r => r.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RequestedBy)
            .WithMany()
            .HasForeignKey(r => r.RequestedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ApprovedBy)
            .WithMany()
            .HasForeignKey(r => r.ApprovedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("idx_requests_status");

        builder.HasIndex(r => r.RequestedById)
            .HasDatabaseName("idx_requests_requested_by");

        builder.HasIndex(r => r.AssetId)
            .HasDatabaseName("idx_requests_asset");
    }
}
