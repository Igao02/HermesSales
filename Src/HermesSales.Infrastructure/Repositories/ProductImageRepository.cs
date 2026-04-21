using HermesSales.Domain.Entities;
using HermesSales.Domain.Repositories;
using HermesSales.Infrastructure.Data;

namespace HermesSales.Infrastructure.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly ApplicationDbContext _context;

    public ProductImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ProductImage productImage, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(productImage, cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
