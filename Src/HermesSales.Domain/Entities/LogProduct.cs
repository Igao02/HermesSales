using HermesSales.Domain.Enum;
using HermesSales.DomainCore.Entities;

namespace HermesSales.Domain.Entities;

public class LogProduct : Entity
{
    public LogProduct()
    {
        //ORM Purpose
    }

    public Guid ProductId { get; set; }
    public Guid? ProductImageId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? ApplicationUserId { get; set; }
    public virtual Product? Product { get; set; }
    public virtual ProductImage? ProductImage { get; set; }
    public ProductLogAction Action { get; set; }

    public LogProduct(Guid productId, Guid? productImageId, string productName, DateTime createdAt, bool isActive, string errorMessage, string? applicationUserId, ProductLogAction action)
    {
        ProductId = productId;
        ProductImageId = productImageId;
        ProductName = productName;
        CreatedAt = createdAt;
        IsActive = isActive;
        ErrorMessage = errorMessage;
        ApplicationUserId = applicationUserId;
        Action = action;
    }
}
