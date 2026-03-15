using HermesSales.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HermesSales.Infrastructure.EntitiesConfigurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(pi => pi.Id);

        builder
            .Property(pi => pi.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .Property(pi => pi.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder
            .HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
