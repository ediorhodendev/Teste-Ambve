using Ambev.DeveloperEvaluation.Domain.Entities.Branches;
using Ambev.DeveloperEvaluation.Domain.Entities.Customers;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Configurations.Sales;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SaleNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.SaleDate).IsRequired();

        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);

        builder.Property(x => x.BranchId).IsRequired();
        builder.Property(x => x.BranchName).IsRequired().HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TotalDiscount).HasPrecision(18, 2).IsRequired();

        // ✅ FKs (hardening)
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // backing field _items
        builder.Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SaleNumber).IsUnique();
        builder.HasIndex(x => x.SaleDate);
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.BranchId);

        builder.Ignore(x => x.DomainEvents);
    }
}
