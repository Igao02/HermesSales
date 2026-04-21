using HermesSales.Domain.Entities;

namespace HermesSales.Domain.Repositories;

public interface IProductImageRepository
{
    Task AddAsync(ProductImage productImage, CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
}
