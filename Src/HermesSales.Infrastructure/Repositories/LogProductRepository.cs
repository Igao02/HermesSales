using HermesSales.Domain.Entities;
using HermesSales.Domain.Repositories;
using HermesSales.Infrastructure.Data;

namespace HermesSales.Infrastructure.Repositories;

public class LogProductRepository : ILogProductRepository
{
    private readonly ApplicationDbContext _context;

    public LogProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(LogProduct logProduct, CancellationToken cancellationToken = default)
    {
        await _context.LogProduct.AddAsync(logProduct, cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
