﻿﻿﻿using HermesSales.Domain.Entities;

namespace HermesSales.Domain.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllWithImagesAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Product>GetById(Guid id, CancellationToken cancellationToken = default);
}
