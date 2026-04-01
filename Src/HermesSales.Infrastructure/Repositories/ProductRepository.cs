using HermesSales.Domain.Entities;
using HermesSales.Domain.Repositories;
using HermesSales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HermesSales.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Product.AddAsync(product, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllWithImagesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Product
            .Include(p => p.Images)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Product
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
