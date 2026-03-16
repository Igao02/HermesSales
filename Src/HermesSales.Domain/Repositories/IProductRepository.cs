using HermesSales.Domain.Entities;

namespace HermesSales.Domain.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
