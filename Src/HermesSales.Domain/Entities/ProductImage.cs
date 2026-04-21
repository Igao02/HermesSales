using HermesSales.DomainCore.Entities;

namespace HermesSales.Domain.Entities;

public class ProductImage : Entity
{
    public ProductImage()
    {
        //ORM Purpose
    }

    public Guid ProductId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Product? Product { get; set; }

    public ProductImage(Guid productId, string fileName, string filePath, DateTime createdAt)
    {
        ProductId = productId;
        FileName = fileName;
        FilePath = filePath;
        CreatedAt = createdAt;
    }
}
