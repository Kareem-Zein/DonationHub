using Application.Abstractions.Interfaces.Repositories;
using Domain.Entities.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories;

internal class GenericRepository<T>(AppDbContext appDbContext) : IGenericRepository<T> where T : EntityBase
{
    protected readonly DbSet<T> Set = appDbContext.Set<T>();
    public async Task AddAsync(T entity, CancellationToken cancellationToken)
        => await Set.AddAsync(entity, cancellationToken);

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await Set.FindAsync([id], cancellationToken);

    public void Remove(T entity)
        => Set.Remove(entity);

    public void Update(T entity)
        => Set.Update(entity);
}
