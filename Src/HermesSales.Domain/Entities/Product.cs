using HermesSales.DomainCore.Entities;

namespace HermesSales.Domain.Entities;

public class Product : Entity
{
    public Product()
    {
        //ORM Purpose
    }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; } = decimal.Zero;
    public int StockQuantity { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string? ApplicationUserId { get; set; }

    public Product(string name, string description, decimal price, int stockQuantity, DateTime createdAt, bool isActive, string applicationUserId)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        CreatedAt = createdAt;
        IsActive = isActive;
        ApplicationUserId = applicationUserId;
    }
}
