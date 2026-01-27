using Ambev.DeveloperEvaluation.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Configurations.Products;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("products");

        b.HasKey(x => x.Id);

        b.Property(x => x.ExternalId)
            .HasMaxLength(80)
            .IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Description)
            .HasMaxLength(500);

        b.Property(x => x.IsActive)
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .IsRequired();

        b.Property(x => x.UpdatedAt);

        b.HasIndex(x => x.ExternalId)
            .IsUnique()
            .HasDatabaseName("ux_products_external_id");

        b.HasIndex(x => x.Name)
            .HasDatabaseName("ix_products_name");


        b.Property(x => x.Price)
          .HasPrecision(18, 2)
          .IsRequired();

    }
}
