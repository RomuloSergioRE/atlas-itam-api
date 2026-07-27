using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Itam.Infrastructure.Data.Configurations;

public sealed class AssetMovementConfiguration : IEntityTypeConfiguration<AssetMovement>
{
    public void Configure(EntityTypeBuilder<AssetMovement> builder)
    {
        builder.ToTable("asset_movements");

        builder.HasKey(m => m.MovementId);

        builder.Property(m => m.MovementId)
            .HasColumnName("movement_id")
            .HasColumnType("uuid");

        builder.Property(m => m.Type)
            .HasColumnName("type")
            .HasColumnType("varchar(20)")
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<MovementType>(v, true));

        builder.Property(m => m.Date)
            .HasColumnName("date")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(m => m.AssetId)
            .HasColumnName("asset_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(m => m.FromUserId)
            .HasColumnName("from_user_id")
            .HasColumnType("uuid");

        builder.Property(m => m.ToUserId)
            .HasColumnName("to_user_id")
            .HasColumnType("uuid");

        builder.Property(m => m.ResponsibleId)
            .HasColumnName("responsible_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(m => m.Observation)
            .HasColumnName("observation")
            .HasColumnType("text");

        builder.Property(m => m.RequestId)
            .HasColumnName("request_id")
            .HasColumnType("uuid");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasOne(m => m.Asset)
            .WithMany()
            .HasForeignKey(m => m.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.FromUser)
            .WithMany()
            .HasForeignKey(m => m.FromUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.ToUser)
            .WithMany()
            .HasForeignKey(m => m.ToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.Responsible)
            .WithMany()
            .HasForeignKey(m => m.ResponsibleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Request)
            .WithMany()
            .HasForeignKey(m => m.RequestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => m.AssetId)
            .HasDatabaseName("idx_movements_asset");

        builder.HasIndex(m => m.FromUserId)
            .HasDatabaseName("idx_movements_from_user");

        builder.HasIndex(m => m.ToUserId)
            .HasDatabaseName("idx_movements_to_user");

        builder.HasIndex(m => m.RequestId)
            .HasDatabaseName("idx_movements_request");

        builder.HasIndex(m => m.CreatedAt)
            .HasDatabaseName("idx_movements_created");
    }
}
