using HermesSales.Domain.Entities;

namespace HermesSales.Domain.Repositories;

public interface ILogProductRepository
{
    Task AddAsync(LogProduct logProduct, CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
}
