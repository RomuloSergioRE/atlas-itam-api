using Atlas.Itam.Domain.Entities;
using Atlas.Itam.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Itam.Infrastructure.Data.Configurations;

public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("assets");

        builder.HasKey(a => a.AssetId);

        builder.Property(a => a.AssetId)
            .HasColumnName("asset_id")
            .HasColumnType("uuid");

        builder.Property(a => a.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(200)")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.PatrimonyNumber)
            .HasColumnName("patrimony_number")
            .HasColumnType("varchar(50)")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.SerialNumber)
            .HasColumnName("serial_number")
            .HasColumnType("varchar(100)")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.AcquisitionDate)
            .HasColumnName("acquisition_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(a => a.AcquisitionValue)
            .HasColumnName("acquisition_value")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.Property(a => a.Supplier)
            .HasColumnName("supplier")
            .HasColumnType("varchar(150)")
            .HasMaxLength(150);

        builder.Property(a => a.WarrantyUntil)
            .HasColumnName("warranty_until")
            .HasColumnType("date");

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue(AssetStatus.Available)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<AssetStatus>(v, true));

        builder.Property(a => a.Observations)
            .HasColumnName("observations")
            .HasColumnType("text");

        builder.Property(a => a.CategoryId)
            .HasColumnName("category_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(a => a.LocationId)
            .HasColumnName("location_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(a => a.CurrentUserId)
            .HasColumnName("current_user_id")
            .HasColumnType("uuid");

        builder.Property(a => a.IsDeleted)
            .HasColumnName("is_deleted")
            .HasColumnType("boolean")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasOne(a => a.Category)
            .WithMany()
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Location)
            .WithMany()
            .HasForeignKey(a => a.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CurrentUser)
            .WithMany()
            .HasForeignKey(a => a.CurrentUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => a.PatrimonyNumber)
            .IsUnique()
            .HasDatabaseName("uk_assets_patrimony");

        builder.HasIndex(a => a.SerialNumber)
            .IsUnique()
            .HasDatabaseName("uk_assets_serial");

        builder.HasIndex(a => a.Status)
            .HasDatabaseName("idx_assets_status");

        builder.HasIndex(a => a.CategoryId)
            .HasDatabaseName("idx_assets_category");

        builder.HasIndex(a => a.LocationId)
            .HasDatabaseName("idx_assets_location");

        builder.HasIndex(a => a.CurrentUserId)
            .HasDatabaseName("idx_assets_current_user");

        builder.HasIndex(a => a.IsDeleted)
            .HasDatabaseName("idx_assets_not_deleted")
            .HasFilter("is_deleted = FALSE");
    }
}
