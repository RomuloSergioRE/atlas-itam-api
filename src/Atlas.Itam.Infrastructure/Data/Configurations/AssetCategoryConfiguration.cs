using Atlas.Itam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Itam.Infrastructure.Data.Configurations;

public sealed class AssetCategoryConfiguration : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.ToTable("asset_categories");

        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.CategoryId)
            .HasColumnName("category_id")
            .HasColumnType("uuid");

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(100)")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255);

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .HasColumnType("boolean")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("uk_asset_categories_name");
    }
}
