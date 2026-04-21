using HermesSales.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HermesSales.Infrastructure.EntitiesConfigurations;

internal class LogProductConfiguration : IEntityTypeConfiguration<LogProduct>
{
    public void Configure(EntityTypeBuilder<LogProduct> builder)
    {
        builder.HasKey(lp => lp.Id);

        builder
            .Property(lp => lp.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .Property(lp => lp.CreatedAt)
            .IsRequired();

        builder
            .Property(lp => lp.IsActive)
            .IsRequired();

        builder
            .Property(lp => lp.ErrorMessage)
            .IsRequired();

        builder
            .Property(lp => lp.Action)
            .HasConversion<string>() // ← salva "Created", "Updated" ao invés de 0, 1, 2...
            .IsRequired();

        // Relacionamento com Product
        builder
            .HasOne(lp => lp.Product)
            .WithMany()
            .HasForeignKey(lp => lp.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // ← não deleta o log se o produto for deletado

        // Relacionamento com ProductImage
        builder
            .HasOne(lp => lp.ProductImage)
            .WithMany()
            .HasForeignKey(lp => lp.ProductImageId)
            .OnDelete(DeleteBehavior.SetNull); // ← se a imagem for deletada, só seta null
    }
}