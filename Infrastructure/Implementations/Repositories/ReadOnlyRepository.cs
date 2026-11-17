using Application.Abstractions.Interfaces.Repositories;
using Domain.Entities.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Implementations.Repositories;

internal class ReadOnlyRepository<T>(AppDbContext appDbContext) : IReadOnlyRepository<T> where T : EntityBase
{
    private readonly DbSet<T> _set = appDbContext.Set<T>();
    private IQueryable<T> _defaultQuery => _set.AsNoTracking();

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        => await GetQuery(predicate).AnyAsync(cancellationToken);

    public async Task<bool> AnyAsync(CancellationToken cancellationToken)
        => await GetQuery().AnyAsync(cancellationToken);

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        => await GetQuery(predicate).CountAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken)
        => await GetQuery().CountAsync(cancellationToken);

    public async Task<TSelector?> FirstOrDefaultAsync<TSelector>(Expression<Func<T, TSelector>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        => await GetQuery(predicate).Select(selector).FirstOrDefaultAsync(cancellationToken);

    public async Task<TSelector?> FirstOrDefaultAsync<TSelector>(Expression<Func<T, TSelector>> selector, CancellationToken cancellationToken)
        => await GetQuery().Select(selector).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<TSelector>> ListAsync<TSelector>(Expression<Func<T, TSelector>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        => await GetQuery(predicate).Select(selector).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TSelector>> ListAsync<TSelector>(Expression<Func<T, TSelector>> selector, CancellationToken cancellationToken)
        => await GetQuery().Select(selector).ToListAsync(cancellationToken);

    private IQueryable<T> GetQuery(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate is null)
            return _defaultQuery;

        return _defaultQuery.Where(predicate);
    }
}
